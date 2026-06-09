using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreasuryTransfers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LedgerTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SourceAccountId = table.Column<string>(type: "text", nullable: false),
                    TargetAccountId = table.Column<string>(type: "text", nullable: false),
                    AmountDebited = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AmountCredited = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerTransactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_OperationId",
                table: "LedgerTransactions",
                column: "OperationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LedgerTransactions");
        }
    }
}
