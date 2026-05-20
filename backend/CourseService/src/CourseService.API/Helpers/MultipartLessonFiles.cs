using CourseService.Application.Courses.Dtos;

namespace CourseService.API.Helpers;

public static class MultipartLessonFiles
{
    private const string Prefix = "lessonFile_";

    public static async Task<IReadOnlyDictionary<string, LessonFileUpload>> ParseAsync(
        IFormFileCollection files,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, LessonFileUpload>(StringComparer.Ordinal);

        foreach (var file in files)
        {
            if (!file.Name.StartsWith(Prefix, StringComparison.Ordinal) || file.Length <= 0)
            {
                continue;
            }

            var fileKey = file.Name[Prefix.Length..];
            if (string.IsNullOrWhiteSpace(fileKey))
            {
                continue;
            }

            var stream = new MemoryStream();
            await file.CopyToAsync(stream, cancellationToken);
            stream.Position = 0;

            result[fileKey] = new LessonFileUpload(
                fileKey,
                stream,
                file.FileName,
                file.ContentType,
                file.Length);
        }

        return result;
    }
}
