using Microsoft.EntityFrameworkCore.Migrations;

namespace EfMigrationsApp.Migrations
{
    public partial class ResourceForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Resources_ResourceID",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Apps_Key",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "Modifier",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "Apps");

            migrationBuilder.AddColumn<int>(
                name: "ModCategoryID",
                table: "ResourceGroups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModifierID",
                table: "Requests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Apps",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ModifierCategories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModifierCategories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ModifierCategories_Apps_AppID",
                        column: x => x.AppID,
                        principalTable: "Apps",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModifierCategoryAdmins",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModCategoryID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModifierCategoryAdmins", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ModifierCategoryAdmins_ModifierCategories_ModCategoryID",
                        column: x => x.ModCategoryID,
                        principalTable: "ModifierCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModifierCategoryAdmins_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Modifiers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupID = table.Column<int>(nullable: false),
                    CategoryID = table.Column<int>(nullable: false),
                    ModKey = table.Column<string>(maxLength: 100, nullable: true),
                    TargetKey = table.Column<string>(maxLength: 100, nullable: true),
                    DisplayText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modifiers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Modifiers_ModifierCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "ModifierCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserModifiers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    ModifierID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModifiers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserModifiers_Modifiers_ModifierID",
                        column: x => x.ModifierID,
                        principalTable: "Modifiers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserModifiers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceGroups_ModCategoryID",
                table: "ResourceGroups",
                column: "ModCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ModifierID",
                table: "Requests",
                column: "ModifierID");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_Name",
                table: "Apps",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ModifierCategories_AppID",
                table: "ModifierCategories",
                column: "AppID");

            migrationBuilder.CreateIndex(
                name: "IX_ModifierCategories_Name",
                table: "ModifierCategories",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ModifierCategoryAdmins_UserID",
                table: "ModifierCategoryAdmins",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ModifierCategoryAdmins_ModCategoryID_UserID",
                table: "ModifierCategoryAdmins",
                columns: new[] { "ModCategoryID", "UserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modifiers_ModKey",
                table: "Modifiers",
                column: "ModKey",
                unique: true,
                filter: "[ModKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Modifiers_CategoryID_TargetKey",
                table: "Modifiers",
                columns: new[] { "CategoryID", "TargetKey" },
                unique: true,
                filter: "[TargetKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserModifiers_ModifierID",
                table: "UserModifiers",
                column: "ModifierID");

            migrationBuilder.CreateIndex(
                name: "IX_UserModifiers_UserID",
                table: "UserModifiers",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Modifiers_ModifierID",
                table: "Requests",
                column: "ModifierID",
                principalTable: "Modifiers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Resources_ResourceID",
                table: "Requests",
                column: "ResourceID",
                principalTable: "Resources",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceGroups_ModifierCategories_ModCategoryID",
                table: "ResourceGroups",
                column: "ModCategoryID",
                principalTable: "ModifierCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Modifiers_ModifierID",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Resources_ResourceID",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceGroups_ModifierCategories_ModCategoryID",
                table: "ResourceGroups");

            migrationBuilder.DropTable(
                name: "ModifierCategoryAdmins");

            migrationBuilder.DropTable(
                name: "UserModifiers");

            migrationBuilder.DropTable(
                name: "Modifiers");

            migrationBuilder.DropTable(
                name: "ModifierCategories");

            migrationBuilder.DropIndex(
                name: "IX_ResourceGroups_ModCategoryID",
                table: "ResourceGroups");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ModifierID",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Apps_Name",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "ModCategoryID",
                table: "ResourceGroups");

            migrationBuilder.DropColumn(
                name: "ModifierID",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Apps");

            migrationBuilder.AddColumn<string>(
                name: "Modifier",
                table: "UserRoles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Apps",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Apps_Key",
                table: "Apps",
                column: "Key",
                unique: true,
                filter: "[Key] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Resources_ResourceID",
                table: "Requests",
                column: "ResourceID",
                principalTable: "Resources",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
