using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Master.Data.Migrations
{
    public partial class AddedDisplayNameInApplicationHostTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppDisplayName",
                table: "tblApplicationHosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppDisplayName",
                table: "tblApplicationHosts");
        }
    }
}
