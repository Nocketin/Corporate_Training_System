using LearningService.Application.Dtos;
using MediatR;

namespace LearningService.Application.Learning.Queries;

public record GetCourseProgressQuery(Guid CourseId, Guid UserId) : IRequest<CourseProgressDto>;
