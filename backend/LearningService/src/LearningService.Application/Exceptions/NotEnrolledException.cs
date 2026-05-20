namespace LearningService.Application.Exceptions;

public class NotEnrolledException : Exception
{
    public NotEnrolledException(Guid userId, Guid courseId)
        : base($"User {userId} is not enrolled in course {courseId}.")
    {
        UserId = userId;
        CourseId = courseId;
    }

    public Guid UserId { get; }
    public Guid CourseId { get; }
}
