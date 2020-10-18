using Microsoft.EntityFrameworkCore.Migrations;

namespace EfMigrationsApp.Migrations
{
    public partial class AppTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Apps",
                maxLength: 100,
                nullable: true,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_Key",
                table: "Apps",
                column: "Key",
                unique: true,
                filter: "[Key] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Apps_Key",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Apps");
        }
    }
}
