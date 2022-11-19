using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Master.Data.Migrations
{
    public partial class AuditColumnsAddedInAppServersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "tblAppServers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "tblAppServers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "tblAppServers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "tblAppServers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "tblAppServers");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "tblAppServers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "tblAppServers");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "tblAppServers");
        }
    }
}
