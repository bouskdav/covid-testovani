using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations.SqlServer.Migrations
{
    public partial class internalurl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InternalURL",
                table: "Sources",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalURL",
                table: "Sources");
        }
    }
}
