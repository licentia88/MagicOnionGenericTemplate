using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AUDIT_BASE",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AB_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AB_TYPE = table.Column<int>(type: "int", nullable: false),
                    AB_USER_ID = table.Column<int>(type: "int", nullable: false),
                    AB_SERVICE = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AB_METHOD = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AB_END_POINT = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_BASE", x => x.AB_ROWID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AUTHORIZATIONS_BASE",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AB_NAME = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AB_AUTH_TYPE = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUTHORIZATIONS_BASE", x => x.AB_ROWID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TestModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescriptionDetails = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CheckData = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestModel", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    U_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    U_FULLNAME = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U_USERNAME = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U_IS_ACTIVE = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    U_PASSWORD = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U_NAME = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U_LASTNAME = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U_PHONE_NUMBER = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U_EMAIL = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U_IS_ADMIN = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.U_ROWID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AUDIT_FAILED",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false),
                    AF_PARAMETERS = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AF_ERROR = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_FAILED", x => x.AB_ROWID);
                    table.ForeignKey(
                        name: "FK_AUDIT_FAILED_AUDIT_BASE_AB_ROWID",
                        column: x => x.AB_ROWID,
                        principalTable: "AUDIT_BASE",
                        principalColumn: "AB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AUDIT_QUERY",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false),
                    AQ_PARAMETERS = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_QUERY", x => x.AB_ROWID);
                    table.ForeignKey(
                        name: "FK_AUDIT_QUERY_AUDIT_BASE_AB_ROWID",
                        column: x => x.AB_ROWID,
                        principalTable: "AUDIT_BASE",
                        principalColumn: "AB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AUDIT_RECORDS",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false),
                    AR_TABLE_NAME = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AR_IS_PRIMARYKEY = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AR_PROPERTY_NAME = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AR_OLD_VALUE = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AR_NEW_VALUE = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_RECORDS", x => x.AB_ROWID);
                    table.ForeignKey(
                        name: "FK_AUDIT_RECORDS_AUDIT_BASE_AB_ROWID",
                        column: x => x.AB_ROWID,
                        principalTable: "AUDIT_BASE",
                        principalColumn: "AB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                        principalColumn: "AB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "USER_ROLES",
                columns: table => new
                {
                    UR_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                        name: "FK_USER_ROLES_USERS_UR_USER_REFNO",
                        column: x => x.UR_USER_REFNO,
                        principalTable: "USERS",
                        principalColumn: "U_ROWID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PERMISSIONS",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false),
                    PER_ROLE_REFNO = table.Column<int>(type: "int", nullable: false),
                    PER_PERMISSION_NAME = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERMISSIONS", x => x.AB_ROWID);
                    table.ForeignKey(
                        name: "FK_PERMISSIONS_AUTHORIZATIONS_BASE_AB_ROWID",
                        column: x => x.AB_ROWID,
                        principalTable: "AUTHORIZATIONS_BASE",
                        principalColumn: "AB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PERMISSIONS_ROLES_PER_ROLE_REFNO",
                        column: x => x.PER_ROLE_REFNO,
                        principalTable: "ROLES",
                        principalColumn: "AB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_BASE_AB_DATE_AB_TYPE_AB_USER_ID_AB_SERVICE_AB_METHOD",
                table: "AUDIT_BASE",
                columns: new[] { "AB_DATE", "AB_TYPE", "AB_USER_ID", "AB_SERVICE", "AB_METHOD" });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_RECORDS_AR_TABLE_NAME_AR_PROPERTY_NAME",
                table: "AUDIT_RECORDS",
                columns: new[] { "AR_TABLE_NAME", "AR_PROPERTY_NAME" });

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
                name: "AUDIT_FAILED");

            migrationBuilder.DropTable(
                name: "AUDIT_QUERY");

            migrationBuilder.DropTable(
                name: "AUDIT_RECORDS");

            migrationBuilder.DropTable(
                name: "PERMISSIONS");

            migrationBuilder.DropTable(
                name: "TestModel");

            migrationBuilder.DropTable(
                name: "USER_ROLES");

            migrationBuilder.DropTable(
                name: "AUDIT_BASE");

            migrationBuilder.DropTable(
                name: "ROLES");

            migrationBuilder.DropTable(
                name: "USERS");

            migrationBuilder.DropTable(
                name: "AUTHORIZATIONS_BASE");
        }
    }
}
