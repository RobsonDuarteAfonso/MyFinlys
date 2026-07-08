using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinlys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCardsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardPurchases_Accounts_AccountId",
                table: "CardPurchases");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "CardPurchases",
                newName: "CardId");

            migrationBuilder.RenameIndex(
                name: "IX_CardPurchases_AccountId",
                table: "CardPurchases",
                newName: "IX_CardPurchases_CardId");

            migrationBuilder.CreateTable(
                name: "CreditCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Limit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ClosingDay = table.Column<int>(type: "integer", nullable: false),
                    DueDay = table.Column<int>(type: "integer", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCards_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_AccountId",
                table: "CreditCards",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardPurchases_CreditCards_CardId",
                table: "CardPurchases",
                column: "CardId",
                principalTable: "CreditCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardPurchases_CreditCards_CardId",
                table: "CardPurchases");

            migrationBuilder.DropTable(
                name: "CreditCards");

            migrationBuilder.RenameColumn(
                name: "CardId",
                table: "CardPurchases",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_CardPurchases_CardId",
                table: "CardPurchases",
                newName: "IX_CardPurchases_AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardPurchases_Accounts_AccountId",
                table: "CardPurchases",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
