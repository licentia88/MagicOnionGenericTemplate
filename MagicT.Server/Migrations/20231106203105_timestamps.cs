using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicT.Server.Migrations
{
    public partial class timestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AUDIT_RECORDS_AR_CHANGE_DATE_AR_TABLE_NAME_AR_PROPERTY_NAME",
                table: "AUDIT_RECORDS");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_BASE_AB_USER_ID_AB_TYPE",
                table: "AUDIT_BASE");

            migrationBuilder.DropColumn(
                name: "AR_CHANGE_DATE",
                table: "AUDIT_RECORDS");

            migrationBuilder.AlterColumn<string>(
                name: "AB_SERVICE",
                table: "AUDIT_BASE",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AB_METHOD",
                table: "AUDIT_BASE",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AB_DATE",
                table: "AUDIT_BASE",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_RECORDS_AR_TABLE_NAME_AR_PROPERTY_NAME",
                table: "AUDIT_RECORDS",
                columns: new[] { "AR_TABLE_NAME", "AR_PROPERTY_NAME" });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_BASE_AB_DATE_AB_TYPE_AB_USER_ID_AB_SERVICE_AB_METHOD",
                table: "AUDIT_BASE",
                columns: new[] { "AB_DATE", "AB_TYPE", "AB_USER_ID", "AB_SERVICE", "AB_METHOD" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AUDIT_RECORDS_AR_TABLE_NAME_AR_PROPERTY_NAME",
                table: "AUDIT_RECORDS");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_BASE_AB_DATE_AB_TYPE_AB_USER_ID_AB_SERVICE_AB_METHOD",
                table: "AUDIT_BASE");

            migrationBuilder.DropColumn(
                name: "AB_DATE",
                table: "AUDIT_BASE");

            migrationBuilder.AddColumn<DateTime>(
                name: "AR_CHANGE_DATE",
                table: "AUDIT_RECORDS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "AB_SERVICE",
                table: "AUDIT_BASE",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AB_METHOD",
                table: "AUDIT_BASE",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_RECORDS_AR_CHANGE_DATE_AR_TABLE_NAME_AR_PROPERTY_NAME",
                table: "AUDIT_RECORDS",
                columns: new[] { "AR_CHANGE_DATE", "AR_TABLE_NAME", "AR_PROPERTY_NAME" });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_BASE_AB_USER_ID_AB_TYPE",
                table: "AUDIT_BASE",
                columns: new[] { "AB_USER_ID", "AB_TYPE" });
        }
    }
}
