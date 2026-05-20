namespace LearningService.Application.Exceptions;

public class AlreadyEnrolledException : Exception
{
    public AlreadyEnrolledException(Guid userId, Guid courseId)
        : base($"User {userId} is already enrolled in course {courseId}.")
    {
    }
}
