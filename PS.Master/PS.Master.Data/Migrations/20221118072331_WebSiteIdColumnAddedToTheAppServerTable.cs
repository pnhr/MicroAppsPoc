using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Master.Data.Migrations
{
    public partial class WebSiteIdColumnAddedToTheAppServerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MasterWebSiteId",
                table: "tblAppServers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MasterWebSiteId",
                table: "tblAppServers");
        }
    }
}
