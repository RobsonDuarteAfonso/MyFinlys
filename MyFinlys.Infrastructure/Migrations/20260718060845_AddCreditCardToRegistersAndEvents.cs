using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinlys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCardToRegistersAndEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardId",
                table: "Registers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registers_CreditCardId",
                table: "Registers",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreditCardId",
                table: "Events",
                column: "CreditCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_CreditCards_CreditCardId",
                table: "Events",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registers_CreditCards_CreditCardId",
                table: "Registers",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_CreditCards_CreditCardId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Registers_CreditCards_CreditCardId",
                table: "Registers");

            migrationBuilder.DropIndex(
                name: "IX_Registers_CreditCardId",
                table: "Registers");

            migrationBuilder.DropIndex(
                name: "IX_Events_CreditCardId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreditCardId",
                table: "Registers");

            migrationBuilder.DropColumn(
                name: "CreditCardId",
                table: "Events");
        }
    }
}
