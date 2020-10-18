using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfMigrationsApp.Migrations
{
    public partial class AddAppsAndVersions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceName",
                table: "Requests");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Requests",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VersionID",
                table: "Requests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Apps",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(maxLength: 50, nullable: true),
                    TimeAdded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apps", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestID = table.Column<int>(nullable: false),
                    Severity = table.Column<int>(nullable: false),
                    Caption = table.Column<string>(maxLength: 1000, nullable: true),
                    Message = table.Column<string>(maxLength: 5000, nullable: true),
                    Detail = table.Column<string>(maxLength: 32000, nullable: true),
                    TimeOccurred = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Events_Requests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "Requests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Versions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppID = table.Column<int>(nullable: false),
                    Major = table.Column<int>(nullable: false),
                    Minor = table.Column<int>(nullable: false),
                    Patch = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TimeAdded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Versions_Apps_AppID",
                        column: x => x.AppID,
                        principalTable: "Apps",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_VersionID",
                table: "Requests",
                column: "VersionID");

            migrationBuilder.CreateIndex(
                name: "IX_Events_RequestID",
                table: "Events",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_AppID",
                table: "Versions",
                column: "AppID");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Versions_VersionID",
                table: "Requests",
                column: "VersionID",
                principalTable: "Versions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Versions_VersionID",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Versions");

            migrationBuilder.DropTable(
                name: "Apps");

            migrationBuilder.DropIndex(
                name: "IX_Requests_VersionID",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "VersionID",
                table: "Requests");

            migrationBuilder.AddColumn<string>(
                name: "ResourceName",
                table: "Requests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
