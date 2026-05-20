namespace CourseService.Application.Courses.Services;

public static class LessonFileValidator
{
  public const long MaxFileSizeBytes = 20 * 1024 * 1024;

  private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
  {
    ".pdf", ".doc", ".docx"
  };

  private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
  {
    "application/pdf",
    "application/msword",
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "application/octet-stream"
  };

  public static bool IsAllowed(string fileName, string? contentType, long length, out string? error)
  {
    if (length <= 0)
    {
      error = "File is empty.";
      return false;
    }

    if (length > MaxFileSizeBytes)
    {
      error = $"File exceeds maximum size of {MaxFileSizeBytes / (1024 * 1024)} MB.";
      return false;
    }

    var ext = Path.GetExtension(fileName);
    if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext))
    {
      error = "Only PDF and Word (.doc, .docx) files are allowed.";
      return false;
    }

    if (!string.IsNullOrWhiteSpace(contentType) && !AllowedContentTypes.Contains(contentType))
    {
      error = "Unsupported file content type.";
      return false;
    }

    error = null;
    return true;
  }
}
