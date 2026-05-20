using LearningService.Domain.Enums;

namespace LearningService.Application.Dtos;

public record CompleteLessonResultDto(
    Guid LessonId,
    bool CourseCompleted,
    EnrollmentStatus EnrollmentStatus,
    int CompletedLessons,
    int TotalLessons);
