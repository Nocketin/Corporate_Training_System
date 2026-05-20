using LearningService.Application.Dtos;
using MediatR;

namespace LearningService.Application.Learning.Queries;

public record GetMyEnrollmentsQuery(Guid UserId) : IRequest<IReadOnlyList<MyEnrollmentDto>>;
