using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class SeedDemoCourses : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(CourseDemoSeedSql.Up);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(CourseDemoSeedSql.Down);
    }
}
