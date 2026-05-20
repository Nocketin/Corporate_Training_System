namespace CourseService.Application.Abstractions;

public interface IObjectStorage
{
    /// <summary>Uploads a file and returns bucket name and object key.</summary>
    Task<(string Bucket, string Key)> PutAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);

    Task<(Stream Content, string ContentType)?> GetAsync(
        string bucket,
        string key,
        CancellationToken cancellationToken);

    Task DeleteAsync(string bucket, string key, CancellationToken cancellationToken);
}
