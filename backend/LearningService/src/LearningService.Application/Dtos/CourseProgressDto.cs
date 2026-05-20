using LearningService.Domain.Enums;

namespace LearningService.Application.Dtos;

public record CourseProgressDto(
    Guid CourseId,
    bool IsEnrolled,
    EnrollmentStatus? Status,
    int TotalLessons,
    int CompletedLessons,
    double ProgressPercent,
    IReadOnlyList<Guid> CompletedLessonIds,
    IReadOnlyList<Guid> LockedLessonIds,
    Guid? NextLessonId);
