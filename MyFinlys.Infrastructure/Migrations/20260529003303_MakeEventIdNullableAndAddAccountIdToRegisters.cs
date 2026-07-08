using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinlys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeEventIdNullableAndAddAccountIdToRegisters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "Registers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Registers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Registers_AccountId",
                table: "Registers",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registers_Accounts_AccountId",
                table: "Registers",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registers_Accounts_AccountId",
                table: "Registers");

            migrationBuilder.DropIndex(
                name: "IX_Registers_AccountId",
                table: "Registers");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Registers");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "Registers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
