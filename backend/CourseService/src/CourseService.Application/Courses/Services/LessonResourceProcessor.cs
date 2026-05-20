using CourseService.Application.Abstractions;
using CourseService.Application.Courses.Dtos;
using CourseService.Domain.Entities;

namespace CourseService.Application.Courses.Services;

public class LessonResourceProcessor
{
    private readonly IObjectStorage _objectStorage;

    public LessonResourceProcessor(IObjectStorage objectStorage)
    {
        _objectStorage = objectStorage;
    }

    public async Task ApplyResourcesAsync(
        Lesson lesson,
        IReadOnlyList<CreateLessonResourceRequest>? resources,
        IReadOnlyDictionary<string, LessonFileUpload> lessonFiles,
        CancellationToken cancellationToken)
    {
        if (resources is not { Count: > 0 })
        {
            return;
        }

        foreach (var r in resources.OrderBy(x => x.Order))
        {
            var kind = ParseKind(r.Kind);
            if (kind == LessonResourceKind.Link)
            {
                lesson.Resources.Add(new LessonResource
                {
                    Lesson = lesson,
                    Kind = LessonResourceKind.Link,
                    Title = r.Title.Trim(),
                    Order = r.Order,
                    Url = r.Url?.Trim()
                });
                continue;
            }

            if (string.IsNullOrWhiteSpace(r.FileKey) || !lessonFiles.TryGetValue(r.FileKey, out var file))
            {
                throw new InvalidOperationException($"Lesson file '{r.FileKey}' was not uploaded.");
            }

            if (!LessonFileValidator.IsAllowed(file.FileName, file.ContentType, file.Length, out var error))
            {
                throw new InvalidOperationException(error);
            }

            var (bucket, key) = await _objectStorage.PutAsync(
                file.Content,
                file.FileName,
                file.ContentType ?? "application/octet-stream",
                cancellationToken);

            lesson.Resources.Add(new LessonResource
            {
                Lesson = lesson,
                Kind = LessonResourceKind.File,
                Title = r.Title.Trim(),
                Order = r.Order,
                StorageBucket = bucket,
                StorageKey = key,
                FileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType,
                FileSizeBytes = file.Length
            });
        }
    }

    public async Task SyncResourcesAsync(
        Lesson lesson,
        IReadOnlyList<CreateLessonResourceRequest>? resources,
        IReadOnlyDictionary<string, LessonFileUpload> lessonFiles,
        IReadOnlySet<Guid> deletedResourceIds,
        CancellationToken cancellationToken)
    {
        foreach (var existing in lesson.Resources.Where(r => deletedResourceIds.Contains(r.Id)).ToList())
        {
            await DeleteStorageIfNeededAsync(existing, cancellationToken);
            lesson.Resources.Remove(existing);
        }

        if (resources is not { Count: > 0 })
        {
            return;
        }

        var existingById = lesson.Resources.ToDictionary(r => r.Id);

        foreach (var r in resources.OrderBy(x => x.Order))
        {
            var kind = ParseKind(r.Kind);

            if (r.Id is { } resourceId && existingById.TryGetValue(resourceId, out var existing))
            {
                existing.Title = r.Title.Trim();
                existing.Order = r.Order;
                if (kind == LessonResourceKind.Link)
                {
                    existing.Kind = LessonResourceKind.Link;
                    existing.Url = r.Url?.Trim();
                    await ClearFileFieldsAsync(existing, cancellationToken);
                }
                else if (!string.IsNullOrWhiteSpace(r.FileKey) && lessonFiles.TryGetValue(r.FileKey, out var file))
                {
                    await ReplaceFileAsync(existing, file, cancellationToken);
                }

                continue;
            }

            if (kind == LessonResourceKind.Link)
            {
                lesson.Resources.Add(new LessonResource
                {
                    Lesson = lesson,
                    Kind = LessonResourceKind.Link,
                    Title = r.Title.Trim(),
                    Order = r.Order,
                    Url = r.Url?.Trim()
                });
                continue;
            }

            if (string.IsNullOrWhiteSpace(r.FileKey) || !lessonFiles.TryGetValue(r.FileKey, out var newFile))
            {
                throw new InvalidOperationException($"New file resource '{r.Title}' requires an uploaded file.");
            }

            if (!LessonFileValidator.IsAllowed(newFile.FileName, newFile.ContentType, newFile.Length, out var error))
            {
                throw new InvalidOperationException(error);
            }

            var (bucket, key) = await _objectStorage.PutAsync(
                newFile.Content,
                newFile.FileName,
                newFile.ContentType ?? "application/octet-stream",
                cancellationToken);

            lesson.Resources.Add(new LessonResource
            {
                Lesson = lesson,
                Kind = LessonResourceKind.File,
                Title = r.Title.Trim(),
                Order = r.Order,
                StorageBucket = bucket,
                StorageKey = key,
                FileName = Path.GetFileName(newFile.FileName),
                ContentType = newFile.ContentType,
                FileSizeBytes = newFile.Length
            });
        }
    }

    public async Task DeleteResourcesAsync(
        IEnumerable<LessonResource> resources,
        CancellationToken cancellationToken)
    {
        foreach (var resource in resources)
        {
            await DeleteStorageIfNeededAsync(resource, cancellationToken);
        }
    }

    private async Task ReplaceFileAsync(LessonResource existing, LessonFileUpload file, CancellationToken cancellationToken)
    {
        if (!LessonFileValidator.IsAllowed(file.FileName, file.ContentType, file.Length, out var error))
        {
            throw new InvalidOperationException(error);
        }

        await DeleteStorageIfNeededAsync(existing, cancellationToken);

        var (bucket, key) = await _objectStorage.PutAsync(
            file.Content,
            file.FileName,
            file.ContentType ?? "application/octet-stream",
            cancellationToken);

        existing.Kind = LessonResourceKind.File;
        existing.StorageBucket = bucket;
        existing.StorageKey = key;
        existing.FileName = Path.GetFileName(file.FileName);
        existing.ContentType = file.ContentType;
        existing.FileSizeBytes = file.Length;
        existing.Url = null;
    }

    private async Task ClearFileFieldsAsync(LessonResource existing, CancellationToken cancellationToken)
    {
        await DeleteStorageIfNeededAsync(existing, cancellationToken);
        existing.StorageBucket = null;
        existing.StorageKey = null;
        existing.FileName = null;
        existing.ContentType = null;
        existing.FileSizeBytes = null;
    }

    private async Task DeleteStorageIfNeededAsync(LessonResource resource, CancellationToken cancellationToken)
    {
        if (resource.Kind == LessonResourceKind.File
            && !string.IsNullOrWhiteSpace(resource.StorageBucket)
            && !string.IsNullOrWhiteSpace(resource.StorageKey))
        {
            await _objectStorage.DeleteAsync(resource.StorageBucket, resource.StorageKey, cancellationToken);
        }
    }

    private static LessonResourceKind ParseKind(string kind) =>
        kind.Equals("file", StringComparison.OrdinalIgnoreCase)
            ? LessonResourceKind.File
            : LessonResourceKind.Link;
}
