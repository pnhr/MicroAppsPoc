using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Master.Logging.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblActivityTypes",
                columns: table => new
                {
                    ActivityTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ActivityTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblActivityTypes", x => x.ActivityTypeId);
                });

            migrationBuilder.CreateTable(
                name: "tblErrorLog",
                columns: table => new
                {
                    ErrorId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", nullable: false),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MethodName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlOrModule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogLevelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblErrorLog", x => x.ErrorId);
                });

            migrationBuilder.CreateTable(
                name: "tblErrorTypes",
                columns: table => new
                {
                    ErrorTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ErrorTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblErrorTypes", x => x.ErrorTypeId);
                });

            migrationBuilder.CreateTable(
                name: "tblLogLevels",
                columns: table => new
                {
                    LogLevelId = table.Column<long>(type: "bigint", nullable: false),
                    Level = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblLogLevels", x => x.LogLevelId);
                });

            migrationBuilder.CreateTable(
                name: "tblActivityLog",
                columns: table => new
                {
                    ActivityId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UrlOrModule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogLevelId = table.Column<long>(type: "bigint", nullable: true),
                    AppLogLevelLogLevelId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblActivityLog", x => x.ActivityId);
                    table.ForeignKey(
                        name: "FK_tblActivityLog_tblLogLevels_AppLogLevelLogLevelId",
                        column: x => x.AppLogLevelLogLevelId,
                        principalTable: "tblLogLevels",
                        principalColumn: "LogLevelId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblActivityLog_AppLogLevelLogLevelId",
                table: "tblActivityLog",
                column: "AppLogLevelLogLevelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblActivityLog");

            migrationBuilder.DropTable(
                name: "tblActivityTypes");

            migrationBuilder.DropTable(
                name: "tblErrorLog");

            migrationBuilder.DropTable(
                name: "tblErrorTypes");

            migrationBuilder.DropTable(
                name: "tblLogLevels");
        }
    }
}
