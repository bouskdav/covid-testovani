using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace FiremniTestovani.Data.Migrations
{
    public partial class bookingsourceid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceID",
                table: "TimeSlotBookings",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlotBookings_SourceID",
                table: "TimeSlotBookings",
                column: "SourceID");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlotBookings_Sources_SourceID",
                table: "TimeSlotBookings",
                column: "SourceID",
                principalTable: "Sources",
                principalColumn: "SourceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"CREATE VIEW TimeSlotOccupancy AS
                SELECT 
                    TimeSlots.SourceID,
	                TimeSlots.TimeSlotID,
	                TimeSlots.From,
	                TimeSlots.To,
	                TimeSlots.Capacity,
	                SUM(case when TimeSlotBookings.AttendanceCanceled = 0 then 1 else 0 end) as OccupiedSpaceCount
                FROM TimeSlots
                LEFT JOIN TimeSlotBookings
                ON TimeSlots.TimeSlotID = TimeSlotBookings.TimeSlotID
                GROUP by TimeSlotID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlotBookings_Sources_SourceID",
                table: "TimeSlotBookings");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlotBookings_SourceID",
                table: "TimeSlotBookings");

            migrationBuilder.DropColumn(
                name: "SourceID",
                table: "TimeSlotBookings");
        }
    }
}
