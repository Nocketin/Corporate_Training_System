using LearningService.Application.Dtos;
using MediatR;

namespace LearningService.Application.Learning.Commands;

public record CompleteLessonCommand(Guid LessonId, Guid UserId) : IRequest<CompleteLessonResultDto>;
