using LearningService.Domain.Enums;

namespace LearningService.Application.Dtos;

public record EnrollmentDto(
    Guid Id,
    Guid CourseId,
    EnrollmentStatus Status,
    DateTime StartDate,
    DateTime? CompleteDate);
