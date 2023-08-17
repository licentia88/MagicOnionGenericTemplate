using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    /// <inheritdoc />
    public partial class failedTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FAILED_TRANSACTIONS_LOG",
                columns: table => new
                {
                    FTL_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FTL_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FTL_ERROR = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAILED_TRANSACTIONS_LOG", x => x.FTL_ROWID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAILED_TRANSACTIONS_LOG");
        }
    }
}
