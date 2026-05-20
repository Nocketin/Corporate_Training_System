using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using CourseService.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace CourseService.Infrastructure.Storage;

public class MinioS3Options
{
    public string ServiceUrl { get; set; } = "http://localhost:9000";
    public string AccessKey { get; set; } = "minio";
    public string SecretKey { get; set; } = "minio123";
    public string Bucket { get; set; } = "courses";
}

public class MinioS3ObjectStorage : IObjectStorage
{
    private readonly IAmazonS3 _client;
    private readonly string _bucket;
    private readonly SemaphoreSlim _bucketGate = new(1, 1);
    private bool _bucketEnsured;

    public MinioS3ObjectStorage(IOptions<MinioS3Options> options)
    {
        var o = options.Value;
        _bucket = o.Bucket;
        var cfg = new AmazonS3Config
        {
            ServiceURL = o.ServiceUrl,
            ForcePathStyle = true
        };
        _client = new AmazonS3Client(o.AccessKey, o.SecretKey, cfg);
    }

    public async Task<(string Bucket, string Key)> PutAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        await EnsureBucketExistsAsync(cancellationToken).ConfigureAwait(false);

        var safeName = Path.GetFileName(fileName);
        var key = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}_{safeName}";

        var put = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            InputStream = content,
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType
        };

        await _client.PutObjectAsync(put, cancellationToken).ConfigureAwait(false);
        return (_bucket, key);
    }

    public async Task<(Stream Content, string ContentType)?> GetAsync(
        string bucket,
        string key,
        CancellationToken cancellationToken)
    {
        try
        {
            var resp = await _client.GetObjectAsync(bucket, key, cancellationToken).ConfigureAwait(false);
            var ms = new MemoryStream();
            await resp.ResponseStream.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
            ms.Position = 0;
            return (ms, resp.Headers.ContentType ?? "application/octet-stream");
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string bucket, string key, CancellationToken cancellationToken)
    {
        try
        {
            await _client.DeleteObjectAsync(bucket, key, cancellationToken).ConfigureAwait(false);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // already gone
        }
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        if (_bucketEnsured)
        {
            return;
        }

        await _bucketGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_bucketEnsured)
            {
                return;
            }

            var exists = await AmazonS3Util.DoesS3BucketExistV2Async(_client, _bucket).ConfigureAwait(false);
            if (!exists)
            {
                await _client.PutBucketAsync(new PutBucketRequest { BucketName = _bucket }, cancellationToken)
                    .ConfigureAwait(false);
            }

            _bucketEnsured = true;
        }
        finally
        {
            _bucketGate.Release();
        }
    }
}
