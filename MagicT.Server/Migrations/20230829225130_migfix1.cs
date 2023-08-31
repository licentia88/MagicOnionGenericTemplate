using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    public partial class migfix1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_USER_ROLES_AUTHORIZATIONS_BASE_AB_ROWID",
                table: "USER_ROLES");

            migrationBuilder.DropIndex(
                name: "IX_USER_ROLES_AB_ROWID",
                table: "USER_ROLES");

            migrationBuilder.DropColumn(
                name: "AB_ROWID",
                table: "USER_ROLES");

            migrationBuilder.CreateIndex(
                name: "IX_USER_ROLES_UR_AUTH_CODE",
                table: "USER_ROLES",
                column: "UR_AUTH_CODE");

            migrationBuilder.AddForeignKey(
                name: "FK_USER_ROLES_AUTHORIZATIONS_BASE_UR_AUTH_CODE",
                table: "USER_ROLES",
                column: "UR_AUTH_CODE",
                principalTable: "AUTHORIZATIONS_BASE",
                principalColumn: "AB_ROWID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_USER_ROLES_AUTHORIZATIONS_BASE_UR_AUTH_CODE",
                table: "USER_ROLES");

            migrationBuilder.DropIndex(
                name: "IX_USER_ROLES_UR_AUTH_CODE",
                table: "USER_ROLES");

            migrationBuilder.AddColumn<int>(
                name: "AB_ROWID",
                table: "USER_ROLES",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_USER_ROLES_AB_ROWID",
                table: "USER_ROLES",
                column: "AB_ROWID");

            migrationBuilder.AddForeignKey(
                name: "FK_USER_ROLES_AUTHORIZATIONS_BASE_AB_ROWID",
                table: "USER_ROLES",
                column: "AB_ROWID",
                principalTable: "AUTHORIZATIONS_BASE",
                principalColumn: "AB_ROWID");
        }
    }
}
