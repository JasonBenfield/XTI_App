using Microsoft.EntityFrameworkCore.Migrations;

namespace EfMigrationsApp.Migrations
{
    public partial class AddedAppType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Apps",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Apps");
        }
    }
}
