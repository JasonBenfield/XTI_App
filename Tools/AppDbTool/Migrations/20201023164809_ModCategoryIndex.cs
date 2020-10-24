using Microsoft.EntityFrameworkCore.Migrations;

namespace EfMigrationsApp.Migrations
{
    public partial class ModCategoryIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Requests_RequestID",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_ModifierCategories_Apps_AppID",
                table: "ModifierCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ModifierCategoryAdmins_ModifierCategories_ModCategoryID",
                table: "ModifierCategoryAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_ModifierCategoryAdmins_Users_UserID",
                table: "ModifierCategoryAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_Modifiers_ModifierCategories_CategoryID",
                table: "Modifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Modifiers_ModifierID",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Resources_ResourceID",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Sessions_SessionID",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Versions_VersionID",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceGroups_Apps_AppID",
                table: "ResourceGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceGroups_ModifierCategories_ModCategoryID",
                table: "ResourceGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Resources_ResourceGroups_GroupID",
                table: "Resources");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Apps_AppID",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_UserID",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModifiers_Modifiers_ModifierID",
                table: "UserModifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModifiers_Users_UserID",
                table: "UserModifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleID",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserID",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Apps_AppID",
                table: "Versions");

            migrationBuilder.DropIndex(
                name: "IX_ModifierCategories_AppID",
                table: "ModifierCategories");

            migrationBuilder.DropIndex(
                name: "IX_ModifierCategories_Name",
                table: "ModifierCategories");

            migrationBuilder.CreateIndex(
                name: "IX_ModifierCategories_AppID_Name",
                table: "ModifierCategories",
                columns: new[] { "AppID", "Name" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Requests_RequestID",
                table: "Events",
                column: "RequestID",
                principalTable: "Requests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModifierCategories_Apps_AppID",
                table: "ModifierCategories",
                column: "AppID",
                principalTable: "Apps",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModifierCategoryAdmins_ModifierCategories_ModCategoryID",
                table: "ModifierCategoryAdmins",
                column: "ModCategoryID",
                principalTable: "ModifierCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModifierCategoryAdmins_Users_UserID",
                table: "ModifierCategoryAdmins",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Modifiers_ModifierCategories_CategoryID",
                table: "Modifiers",
                column: "CategoryID",
                principalTable: "ModifierCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_Requests_Sessions_SessionID",
                table: "Requests",
                column: "SessionID",
                principalTable: "Sessions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Versions_VersionID",
                table: "Requests",
                column: "VersionID",
                principalTable: "Versions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceGroups_Apps_AppID",
                table: "ResourceGroups",
                column: "AppID",
                principalTable: "Apps",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceGroups_ModifierCategories_ModCategoryID",
                table: "ResourceGroups",
                column: "ModCategoryID",
                principalTable: "ModifierCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_ResourceGroups_GroupID",
                table: "Resources",
                column: "GroupID",
                principalTable: "ResourceGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Apps_AppID",
                table: "Roles",
                column: "AppID",
                principalTable: "Apps",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_UserID",
                table: "Sessions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModifiers_Modifiers_ModifierID",
                table: "UserModifiers",
                column: "ModifierID",
                principalTable: "Modifiers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModifiers_Users_UserID",
                table: "UserModifiers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleID",
                table: "UserRoles",
                column: "RoleID",
                principalTable: "Roles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserID",
                table: "UserRoles",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Versions_Apps_AppID",
                table: "Versions",
                column: "AppID",
                principalTable: "Apps",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Requests_RequestID",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_ModifierCategories_Apps_AppID",
                table: "ModifierCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ModifierCategoryAdmins_ModifierCategories_ModCategoryID",
                table: "ModifierCategoryAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_ModifierCategoryAdmins_Users_UserID",
                table: "ModifierCategoryAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_Modifiers_ModifierCategories_CategoryID",
                table: "Modifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Modifiers_ModifierID",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Resources_ResourceID",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Sessions_SessionID",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Versions_VersionID",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceGroups_Apps_AppID",
                table: "ResourceGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceGroups_ModifierCategories_ModCategoryID",
                table: "ResourceGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Resources_ResourceGroups_GroupID",
                table: "Resources");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Apps_AppID",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_UserID",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModifiers_Modifiers_ModifierID",
                table: "UserModifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModifiers_Users_UserID",
                table: "UserModifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleID",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserID",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Apps_AppID",
                table: "Versions");

            migrationBuilder.DropIndex(
                name: "IX_ModifierCategories_AppID_Name",
                table: "ModifierCategories");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Requests_RequestID",
                table: "Events",
                column: "RequestID",
                principalTable: "Requests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModifierCategories_Apps_AppID",
                table: "ModifierCategories",
                column: "AppID",
                principalTable: "Apps",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModifierCategoryAdmins_ModifierCategories_ModCategoryID",
                table: "ModifierCategoryAdmins",
                column: "ModCategoryID",
                principalTable: "ModifierCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModifierCategoryAdmins_Users_UserID",
                table: "ModifierCategoryAdmins",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Modifiers_ModifierCategories_CategoryID",
                table: "Modifiers",
                column: "CategoryID",
                principalTable: "ModifierCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Modifiers_ModifierID",
                table: "Requests",
                column: "ModifierID",
                principalTable: "Modifiers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Resources_ResourceID",
                table: "Requests",
                column: "ResourceID",
                principalTable: "Resources",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Sessions_SessionID",
                table: "Requests",
                column: "SessionID",
                principalTable: "Sessions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Versions_VersionID",
                table: "Requests",
                column: "VersionID",
                principalTable: "Versions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceGroups_Apps_AppID",
                table: "ResourceGroups",
                column: "AppID",
                principalTable: "Apps",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceGroups_ModifierCategories_ModCategoryID",
                table: "ResourceGroups",
                column: "ModCategoryID",
                principalTable: "ModifierCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_ResourceGroups_GroupID",
                table: "Resources",
                column: "GroupID",
                principalTable: "ResourceGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Apps_AppID",
                table: "Roles",
                column: "AppID",
                principalTable: "Apps",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_UserID",
                table: "Sessions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModifiers_Modifiers_ModifierID",
                table: "UserModifiers",
                column: "ModifierID",
                principalTable: "Modifiers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModifiers_Users_UserID",
                table: "UserModifiers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleID",
                table: "UserRoles",
                column: "RoleID",
                principalTable: "Roles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserID",
                table: "UserRoles",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Versions_Apps_AppID",
                table: "Versions",
                column: "AppID",
                principalTable: "Apps",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
