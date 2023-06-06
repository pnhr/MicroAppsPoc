using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Master.Data.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblApplicationHosts",
                columns: table => new
                {
                    AppId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppDiscription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppRootPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppVPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblApplicationHosts", x => x.AppId);
                });

            migrationBuilder.CreateTable(
                name: "tblAppServers",
                columns: table => new
                {
                    ServerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeployRootPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MasterWebSiteId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAppServers", x => x.ServerId);
                });

            migrationBuilder.CreateTable(
                name: "tblEmployees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblEmployees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_tblEmployees_tblEmployees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "tblEmployees",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "tblMasterConfig",
                columns: table => new
                {
                    ConfigId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblMasterConfig", x => x.ConfigId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblAppServers_ServerCode",
                table: "tblAppServers",
                column: "ServerCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblEmployees_ManagerId",
                table: "tblEmployees",
                column: "ManagerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblApplicationHosts");

            migrationBuilder.DropTable(
                name: "tblAppServers");

            migrationBuilder.DropTable(
                name: "tblEmployees");

            migrationBuilder.DropTable(
                name: "tblMasterConfig");
        }
    }
}
