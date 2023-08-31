using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    public partial class nefflds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RL_SERVICE_NAME",
                table: "ROLES",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PER_METHOD_NAME",
                table: "PERMISSIONS",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RL_SERVICE_NAME",
                table: "ROLES");

            migrationBuilder.DropColumn(
                name: "PER_METHOD_NAME",
                table: "PERMISSIONS");
        }
    }
}
