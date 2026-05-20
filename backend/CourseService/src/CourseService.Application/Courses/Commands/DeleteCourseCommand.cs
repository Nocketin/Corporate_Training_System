using MediatR;

namespace CourseService.Application.Courses.Commands;

public record DeleteCourseCommand(Guid CourseId) : IRequest;
