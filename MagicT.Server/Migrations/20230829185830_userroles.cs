using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    public partial class userroles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "USER_AUTHORIZATIONS");

            migrationBuilder.CreateTable(
                name: "USER_ROLES",
                columns: table => new
                {
                    UR_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UR_USER_REFNO = table.Column<int>(type: "int", nullable: false),
                    UR_AUTH_CODE = table.Column<int>(type: "int", nullable: false),
                    AB_ROWID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_ROLES", x => x.UR_ROWID);
                    table.ForeignKey(
                        name: "FK_USER_ROLES_AUTHORIZATIONS_BASE_AB_ROWID",
                        column: x => x.AB_ROWID,
                        principalTable: "AUTHORIZATIONS_BASE",
                        principalColumn: "AB_ROWID");
                    table.ForeignKey(
                        name: "FK_USER_ROLES_USERS_BASE_UR_USER_REFNO",
                        column: x => x.UR_USER_REFNO,
                        principalTable: "USERS_BASE",
                        principalColumn: "UB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USER_ROLES_AB_ROWID",
                table: "USER_ROLES",
                column: "AB_ROWID");

            migrationBuilder.CreateIndex(
                name: "IX_USER_ROLES_UR_USER_REFNO",
                table: "USER_ROLES",
                column: "UR_USER_REFNO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "USER_ROLES");

            migrationBuilder.CreateTable(
                name: "USER_AUTHORIZATIONS",
                columns: table => new
                {
                    UA_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AB_ROWID = table.Column<int>(type: "int", nullable: true),
                    UA_AUTH_CODE = table.Column<int>(type: "int", nullable: false),
                    UA_USER_REFNO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_AUTHORIZATIONS", x => x.UA_ROWID);
                    table.ForeignKey(
                        name: "FK_USER_AUTHORIZATIONS_AUTHORIZATIONS_BASE_AB_ROWID",
                        column: x => x.AB_ROWID,
                        principalTable: "AUTHORIZATIONS_BASE",
                        principalColumn: "AB_ROWID");
                    table.ForeignKey(
                        name: "FK_USER_AUTHORIZATIONS_USERS_BASE_UA_USER_REFNO",
                        column: x => x.UA_USER_REFNO,
                        principalTable: "USERS_BASE",
                        principalColumn: "UB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USER_AUTHORIZATIONS_AB_ROWID",
                table: "USER_AUTHORIZATIONS",
                column: "AB_ROWID");

            migrationBuilder.CreateIndex(
                name: "IX_USER_AUTHORIZATIONS_UA_USER_REFNO",
                table: "USER_AUTHORIZATIONS",
                column: "UA_USER_REFNO");
        }
    }
}
