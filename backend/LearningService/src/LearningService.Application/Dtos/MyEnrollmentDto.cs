using LearningService.Domain.Enums;

namespace LearningService.Application.Dtos;

public record MyEnrollmentDto(
    Guid EnrollmentId,
    Guid CourseId,
    string? CourseTitle,
    EnrollmentStatus Status,
    DateTime StartDate,
    DateTime? CompleteDate,
    string? CertificateFileUrl);
