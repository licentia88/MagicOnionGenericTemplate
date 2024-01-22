using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    public partial class flndsremove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckData2",
                table: "TestModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckData3",
                table: "TestModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckData4",
                table: "TestModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckData5",
                table: "TestModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckData6",
                table: "TestModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckData7",
                table: "TestModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckData8",
                table: "TestModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckData9",
                table: "TestModel",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckData2",
                table: "TestModel");

            migrationBuilder.DropColumn(
                name: "CheckData3",
                table: "TestModel");

            migrationBuilder.DropColumn(
                name: "CheckData4",
                table: "TestModel");

            migrationBuilder.DropColumn(
                name: "CheckData5",
                table: "TestModel");

            migrationBuilder.DropColumn(
                name: "CheckData6",
                table: "TestModel");

            migrationBuilder.DropColumn(
                name: "CheckData7",
                table: "TestModel");

            migrationBuilder.DropColumn(
                name: "CheckData8",
                table: "TestModel");

            migrationBuilder.DropColumn(
                name: "CheckData9",
                table: "TestModel");
        }
    }
}
