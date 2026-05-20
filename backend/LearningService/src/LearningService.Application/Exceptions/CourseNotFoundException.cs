namespace LearningService.Application.Exceptions;

public class CourseNotFoundException : Exception
{
    public CourseNotFoundException(Guid courseId)
        : base($"Course {courseId} was not found.")
    {
        CourseId = courseId;
    }

    public Guid CourseId { get; }
}
