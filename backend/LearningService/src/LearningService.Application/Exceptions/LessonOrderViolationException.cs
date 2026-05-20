namespace LearningService.Application.Exceptions;

public class LessonOrderViolationException : Exception
{
    public LessonOrderViolationException(string message) : base(message)
    {
    }
}
