using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    /// <inheritdoc />
    public partial class permissionsDesc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PERMISSIONS_ROLES_PER_ROLE_REFNO",
                table: "PERMISSIONS");

            migrationBuilder.AlterColumn<int>(
                name: "PER_ROLE_REFNO",
                table: "PERMISSIONS",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AB_DESCRIPTION",
                table: "AUTHORIZATIONS_BASE",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PERMISSIONS_ROLES_PER_ROLE_REFNO",
                table: "PERMISSIONS",
                column: "PER_ROLE_REFNO",
                principalTable: "ROLES",
                principalColumn: "AB_ROWID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PERMISSIONS_ROLES_PER_ROLE_REFNO",
                table: "PERMISSIONS");

            migrationBuilder.DropColumn(
                name: "AB_DESCRIPTION",
                table: "AUTHORIZATIONS_BASE");

            migrationBuilder.AlterColumn<int>(
                name: "PER_ROLE_REFNO",
                table: "PERMISSIONS",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PERMISSIONS_ROLES_PER_ROLE_REFNO",
                table: "PERMISSIONS",
                column: "PER_ROLE_REFNO",
                principalTable: "ROLES",
                principalColumn: "AB_ROWID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
