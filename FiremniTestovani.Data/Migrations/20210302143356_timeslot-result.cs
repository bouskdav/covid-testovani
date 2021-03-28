using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations
{
    public partial class timeslotresult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "TestResult",
                table: "TimeSlotBookings",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "TestResult",
                table: "TimeSlotBookings",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
