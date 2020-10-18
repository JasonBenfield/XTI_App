using Microsoft.EntityFrameworkCore.Migrations;

namespace EfMigrationsApp.Migrations
{
    public partial class AddedVersionKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VersionKey",
                table: "Versions",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Versions_VersionKey",
                table: "Versions",
                column: "VersionKey",
                unique: true,
                filter: "[VersionKey] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Versions_VersionKey",
                table: "Versions");

            migrationBuilder.DropColumn(
                name: "VersionKey",
                table: "Versions");
        }
    }
}
