using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations
{
    public partial class bookingfirstnamesurname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "TimeSlotBookings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "TimeSlotBookings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "TimeSlotBookings");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "TimeSlotBookings");
        }
    }
}
