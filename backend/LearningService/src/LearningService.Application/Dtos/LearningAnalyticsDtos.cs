namespace LearningService.Application.Dtos;

public record LearningAnalyticsOverviewDto(
    LearningAnalyticsSummaryDto Summary,
    IReadOnlyList<DailyCountDto> EnrollmentsByDay,
    IReadOnlyList<DailyCountDto> CompletionsByDay,
    IReadOnlyList<CourseActivityDto> CoursesActivity);

public record LearningAnalyticsSummaryDto(
    int TotalEnrollments,
    int CompletedEnrollments,
    int ActiveEnrollments,
    double CompletionRatePercent);

public record DailyCountDto(DateOnly Date, int Count);

public record CourseActivityDto(
    Guid CourseId,
    string Title,
    int Enrollments,
    int Completions);
