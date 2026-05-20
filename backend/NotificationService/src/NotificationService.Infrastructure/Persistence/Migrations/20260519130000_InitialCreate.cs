using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NotificationService.Infrastructure.Persistence;

#nullable disable

namespace NotificationService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(NotificationDbContext))]
[Migration("20260519130000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
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
        migrationBuilder.DropTable(name: "ProcessedInboundMessages");
    }
}
