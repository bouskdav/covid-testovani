using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations.SqlServer.Migrations
{
    public partial class bookingsecuritycode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecurityCode",
                table: "TimeSlotBookings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidationCode",
                table: "TimeSlotBookings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityCode",
                table: "TimeSlotBookings");

            migrationBuilder.DropColumn(
                name: "ValidationCode",
                table: "TimeSlotBookings");
        }
    }
}
