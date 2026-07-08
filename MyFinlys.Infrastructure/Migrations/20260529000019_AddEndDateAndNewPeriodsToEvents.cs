using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinlys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEndDateAndNewPeriodsToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Events",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Events");
        }
    }
}
