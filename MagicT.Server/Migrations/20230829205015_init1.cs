using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    public partial class init1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PERMISSIONS_ROLES_PER_ROLE_REFNO",
                table: "PERMISSIONS");

            migrationBuilder.DropTable(
                name: "ROLES");

            migrationBuilder.DropIndex(
                name: "IX_PERMISSIONS_PER_ROLE_REFNO",
                table: "PERMISSIONS");

            migrationBuilder.DropColumn(
                name: "PER_ROLE_REFNO",
                table: "PERMISSIONS");

            migrationBuilder.DropColumn(
                name: "AB_DESCRIPTION",
                table: "AUTHORIZATIONS_BASE");

            migrationBuilder.RenameColumn(
                name: "PER_METHOD_NAME",
                table: "PERMISSIONS",
                newName: "PER_PERMISSION_NAME");

            migrationBuilder.AddColumn<string>(
                name: "PER_IDENTIFIER_NAME",
                table: "PERMISSIONS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ROLES_M",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLES_M", x => x.AB_ROWID);
                    table.ForeignKey(
                        name: "FK_ROLES_M_AUTHORIZATIONS_BASE_AB_ROWID",
                        column: x => x.AB_ROWID,
                        principalTable: "AUTHORIZATIONS_BASE",
                        principalColumn: "AB_ROWID");
                });

            migrationBuilder.CreateTable(
                name: "ROLES_D",
                columns: table => new
                {
                    RD_ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RD_M_REFNO = table.Column<int>(type: "int", nullable: false),
                    RD_PERMISSION_REFNO = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLES_D", x => x.RD_ROWID);
                    table.ForeignKey(
                        name: "FK_ROLES_D_PERMISSIONS_RD_PERMISSION_REFNO",
                        column: x => x.RD_PERMISSION_REFNO,
                        principalTable: "PERMISSIONS",
                        principalColumn: "AB_ROWID");
                    table.ForeignKey(
                        name: "FK_ROLES_D_ROLES_M_RD_M_REFNO",
                        column: x => x.RD_M_REFNO,
                        principalTable: "ROLES_M",
                        principalColumn: "AB_ROWID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ROLES_D_RD_M_REFNO",
                table: "ROLES_D",
                column: "RD_M_REFNO");

            migrationBuilder.CreateIndex(
                name: "IX_ROLES_D_RD_PERMISSION_REFNO",
                table: "ROLES_D",
                column: "RD_PERMISSION_REFNO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ROLES_D");

            migrationBuilder.DropTable(
                name: "ROLES_M");

            migrationBuilder.DropColumn(
                name: "PER_IDENTIFIER_NAME",
                table: "PERMISSIONS");

            migrationBuilder.RenameColumn(
                name: "PER_PERMISSION_NAME",
                table: "PERMISSIONS",
                newName: "PER_METHOD_NAME");

            migrationBuilder.AddColumn<int>(
                name: "PER_ROLE_REFNO",
                table: "PERMISSIONS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AB_DESCRIPTION",
                table: "AUTHORIZATIONS_BASE",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ROLES",
                columns: table => new
                {
                    AB_ROWID = table.Column<int>(type: "int", nullable: false),
                    RL_SERVICE_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_PERMISSIONS_PER_ROLE_REFNO",
                table: "PERMISSIONS",
                column: "PER_ROLE_REFNO");

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
