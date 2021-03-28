using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations
{
    public partial class sourceearliestnextpossibletest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EarliestNextPossibleTest",
                table: "Sources",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "EarliestNextPossibleTestNumericValue",
                table: "Sources",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarliestNextPossibleTest",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "EarliestNextPossibleTestNumericValue",
                table: "Sources");
        }
    }
}
