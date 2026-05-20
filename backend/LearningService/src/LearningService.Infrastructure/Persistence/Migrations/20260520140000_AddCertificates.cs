using LearningService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(LearningDbContext))]
[Migration("20260520140000_AddCertificates")]
public partial class AddCertificates : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Certificates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                FileUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Certificates", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Certificates_UserId_CourseId",
            table: "Certificates",
            columns: new[] { "UserId", "CourseId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Certificates");
    }
}
