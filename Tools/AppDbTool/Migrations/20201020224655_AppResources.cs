using Microsoft.EntityFrameworkCore.Migrations;

namespace EfMigrationsApp.Migrations
{
    public partial class AppResources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResourceID",
                table: "Requests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ResourceGroups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ResourceGroups_Apps_AppID",
                        column: x => x.AppID,
                        principalTable: "Apps",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Resources_ResourceGroups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "ResourceGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ResourceID",
                table: "Requests",
                column: "ResourceID");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceGroups_AppID_Name",
                table: "ResourceGroups",
                columns: new[] { "AppID", "Name" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_GroupID_Name",
                table: "Resources",
                columns: new[] { "GroupID", "Name" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Resources_ResourceID",
                table: "Requests",
                column: "ResourceID",
                principalTable: "Resources",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Resources_ResourceID",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "ResourceGroups");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ResourceID",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ResourceID",
                table: "Requests");
        }
    }
}
