using System;
using LearningService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(LearningDbContext))]
[Migration("20260519120000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CourseProjections",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                TotalLessons = table.Column<int>(type: "integer", nullable: false),
                OrderedLessonIdsJson = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_CourseProjections", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_CourseProjections_CourseId",
            table: "CourseProjections",
            column: "CourseId",
            unique: true);

        migrationBuilder.CreateTable(
            name: "Enrollments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CompleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Enrollments", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_Enrollments_UserId_CourseId",
            table: "Enrollments",
            columns: new[] { "UserId", "CourseId" },
            unique: true);

        migrationBuilder.CreateTable(
            name: "LessonProgresses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EnrollmentId = table.Column<Guid>(type: "uuid", nullable: false),
                LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LessonProgresses", x => x.Id);
                table.ForeignKey(
                    name: "FK_LessonProgresses_Enrollments_EnrollmentId",
                    column: x => x.EnrollmentId,
                    principalTable: "Enrollments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_LessonProgresses_EnrollmentId_LessonId",
            table: "LessonProgresses",
            columns: new[] { "EnrollmentId", "LessonId" },
            unique: true);

        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Payload = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                DispatchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_OutboxMessages", x => x.Id));

        migrationBuilder.CreateTable(
            name: "ProcessedInboundMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Topic = table.Column<string>(type: "text", nullable: false),
                Partition = table.Column<int>(type: "integer", nullable: false),
                Offset = table.Column<long>(type: "bigint", nullable: false),
                ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_ProcessedInboundMessages", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_ProcessedInboundMessages_Topic_Partition_Offset",
            table: "ProcessedInboundMessages",
            columns: new[] { "Topic", "Partition", "Offset" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "LessonProgresses");
        migrationBuilder.DropTable(name: "OutboxMessages");
        migrationBuilder.DropTable(name: "ProcessedInboundMessages");
        migrationBuilder.DropTable(name: "CourseProjections");
        migrationBuilder.DropTable(name: "Enrollments");
    }
}
