using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    public partial class audits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIT_M",
                columns: table => new
                {
                    AM_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AM_TABLE_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_M", x => x.AM_ROWID);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_TYPES",
                columns: table => new
                {
                    AT_CODE = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AT_DESC = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_TYPES", x => x.AT_CODE);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_D",
                columns: table => new
                {
                    AD_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AD_M_REFNO = table.Column<int>(type: "int", nullable: false),
                    AD_PARENT_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AD_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AD_CURRENT_USER = table.Column<int>(type: "int", nullable: false),
                    AD_PROPERTY_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AD_OLD_VALUE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AD_NEW_VALUE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AD_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_D", x => x.AD_ROWID);
                    table.ForeignKey(
                        name: "FK_AUDIT_D_AUDIT_M_AD_M_REFNO",
                        column: x => x.AD_M_REFNO,
                        principalTable: "AUDIT_M",
                        principalColumn: "AM_ROWID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_D_AD_M_REFNO",
                table: "AUDIT_D",
                column: "AD_M_REFNO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_D");

            migrationBuilder.DropTable(
                name: "AUDIT_TYPES");

            migrationBuilder.DropTable(
                name: "AUDIT_M");
        }
    }
}
