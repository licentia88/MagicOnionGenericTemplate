using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUTHORIZATIONS_BASE",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AB_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AB_AUTH_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUTHORIZATIONS_BASE", x => x.AB_ROWID);
                });

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

            migrationBuilder.CreateTable(
                name: "TestModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USERS_BASE",
                columns: table => new
                {
                    UB_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UB_FULLNAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UB_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UB_IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false),
                    UB_PASSWORD = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS_BASE", x => x.UB_ROWID);
                });

            migrationBuilder.CreateTable(
                name: "ROLES",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLES", x => x.AB_ROWID);
                    table.ForeignKey(
                        name: "FK_ROLES_AUTHORIZATIONS_BASE_AB_ROWID",
                        column: x => x.AB_ROWID,
                        principalTable: "AUTHORIZATIONS_BASE",
                        principalColumn: "AB_ROWID");
                });

            migrationBuilder.CreateTable(
                name: "USER_ROLES",
                columns: table => new
                {
                    UR_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UR_USER_REFNO = table.Column<int>(type: "int", nullable: false),
                    UR_ROLE_REFNO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_ROLES", x => x.UR_ROWID);
                    table.ForeignKey(
                        name: "FK_USER_ROLES_AUTHORIZATIONS_BASE_UR_ROLE_REFNO",
                        column: x => x.UR_ROLE_REFNO,
                        principalTable: "AUTHORIZATIONS_BASE",
                        principalColumn: "AB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_ROLES_USERS_BASE_UR_USER_REFNO",
                        column: x => x.UR_USER_REFNO,
                        principalTable: "USERS_BASE",
                        principalColumn: "UB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    UB_ROWID = table.Column<int>(type: "int", nullable: false),
                    U_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    U_SURNAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    U_PHONE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    U_EMAIL = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.UB_ROWID);
                    table.ForeignKey(
                        name: "FK_USERS_USERS_BASE_UB_ROWID",
                        column: x => x.UB_ROWID,
                        principalTable: "USERS_BASE",
                        principalColumn: "UB_ROWID");
                });

            migrationBuilder.CreateTable(
                name: "PERMISSIONS",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false),
                    PER_ROLE_REFNO = table.Column<int>(type: "int", nullable: false),
                    PER_PERMISSION_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERMISSIONS", x => x.AB_ROWID);
                    table.ForeignKey(
                        name: "FK_PERMISSIONS_AUTHORIZATIONS_BASE_AB_ROWID",
                        column: x => x.AB_ROWID,
                        principalTable: "AUTHORIZATIONS_BASE",
                        principalColumn: "AB_ROWID");
                    table.ForeignKey(
                        name: "FK_PERMISSIONS_ROLES_PER_ROLE_REFNO",
                        column: x => x.PER_ROLE_REFNO,
                        principalTable: "ROLES",
                        principalColumn: "AB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PERMISSIONS_PER_ROLE_REFNO",
                table: "PERMISSIONS",
                column: "PER_ROLE_REFNO");

            migrationBuilder.CreateIndex(
                name: "IX_USER_ROLES_UR_ROLE_REFNO",
                table: "USER_ROLES",
                column: "UR_ROLE_REFNO");

            migrationBuilder.CreateIndex(
                name: "IX_USER_ROLES_UR_USER_REFNO",
                table: "USER_ROLES",
                column: "UR_USER_REFNO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAILED_TRANSACTIONS_LOG");

            migrationBuilder.DropTable(
                name: "PERMISSIONS");

            migrationBuilder.DropTable(
                name: "TestModel");

            migrationBuilder.DropTable(
                name: "USER_ROLES");

            migrationBuilder.DropTable(
                name: "USERS");

            migrationBuilder.DropTable(
                name: "ROLES");

            migrationBuilder.DropTable(
                name: "USERS_BASE");

            migrationBuilder.DropTable(
                name: "AUTHORIZATIONS_BASE");
        }
    }
}
