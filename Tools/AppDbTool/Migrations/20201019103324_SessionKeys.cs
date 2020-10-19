using Microsoft.EntityFrameworkCore.Migrations;

namespace EfMigrationsApp.Migrations
{
    public partial class SessionKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionKey",
                table: "Sessions",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestKey",
                table: "Requests",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventKey",
                table: "Events",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionKey",
                table: "Sessions",
                column: "SessionKey",
                unique: true,
                filter: "[SessionKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestKey",
                table: "Requests",
                column: "RequestKey",
                unique: true,
                filter: "[RequestKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventKey",
                table: "Events",
                column: "EventKey",
                unique: true,
                filter: "[EventKey] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_SessionKey",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestKey",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventKey",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SessionKey",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "RequestKey",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "EventKey",
                table: "Events");
        }
    }
}
