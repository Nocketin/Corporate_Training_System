using LearningService.Application.Dtos;
using MediatR;

namespace LearningService.Application.Learning.Commands;

public record EnrollInCourseCommand(Guid CourseId, Guid UserId) : IRequest<EnrollmentDto>;
