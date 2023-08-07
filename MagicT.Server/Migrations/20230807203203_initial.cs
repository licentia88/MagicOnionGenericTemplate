using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUTHORIZATIONS_BASE",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AB_USER_REFNO = table.Column<int>(type: "int", nullable: false),
                    AB_AUTH_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AB_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUTHORIZATIONS_BASE", x => x.AB_ROWID);
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
                    UB_NAME = table.Column<int>(type: "int", nullable: false),
                    UB_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UB_SURNAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UB_IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false),
                    UB_PASSWORD = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS_BASE", x => x.UB_ROWID);
                });

            migrationBuilder.CreateTable(
                name: "PERMISSIONS",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false)
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
                        principalColumn: "AB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    UB_ROWID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.UB_ROWID);
                    table.ForeignKey(
                        name: "FK_USERS_USERS_BASE_UB_ROWID",
                        column: x => x.UB_ROWID,
                        principalTable: "USERS_BASE",
                        principalColumn: "UB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PERMISSIONS");

            migrationBuilder.DropTable(
                name: "ROLES");

            migrationBuilder.DropTable(
                name: "TestModel");

            migrationBuilder.DropTable(
                name: "USERS");

            migrationBuilder.DropTable(
                name: "AUTHORIZATIONS_BASE");

            migrationBuilder.DropTable(
                name: "USERS_BASE");
        }
    }
}
