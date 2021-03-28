using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations
{
    public partial class testdurationdouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TestDuration",
                table: "Sources",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TestDuration",
                table: "Sources",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
