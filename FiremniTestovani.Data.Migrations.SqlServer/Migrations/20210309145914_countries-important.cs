using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations.SqlServer.Migrations
{
    public partial class countriesimportant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsImportant",
                table: "Countries",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsImportant",
                table: "Countries");
        }
    }
}
