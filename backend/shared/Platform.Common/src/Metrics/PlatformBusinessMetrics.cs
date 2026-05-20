using Prometheus;

namespace Platform.Common.Observability;

public static class PlatformBusinessMetrics
{
    private static readonly Counter CoursesCreated = Metrics.CreateCounter(
        "cts_courses_created_total",
        "Total courses created.");

    private static readonly Counter EnrollmentsCreated = Metrics.CreateCounter(
        "cts_enrollments_total",
        "Total course enrollments.");

    private static readonly Counter LessonsCompleted = Metrics.CreateCounter(
        "cts_lessons_completed_total",
        "Total lessons marked complete.");

    public static void RecordCourseCreated() => CoursesCreated.Inc();

    public static void RecordEnrollment() => EnrollmentsCreated.Inc();

    public static void RecordLessonCompleted() => LessonsCompleted.Inc();
}
