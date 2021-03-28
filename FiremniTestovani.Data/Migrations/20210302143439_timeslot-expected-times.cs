using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations
{
    public partial class timeslotexpectedtimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ToExpected",
                table: "TimeSlotBookings",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FromExpected",
                table: "TimeSlotBookings",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.Sql(@"CREATE VIEW DateCapacity AS
                SELECT 
	                TimeSlots.SourceID,
	                TimeSlots.Date, 
	                SUM(Capacity) as Capacity
                FROM TimeSlots
                GROUP BY TimeSlots.SourceID, TimeSlots.Date");

            migrationBuilder.Sql(@"CREATE VIEW DateOccupancy AS
                SELECT 
	                TimeSlots.SourceID,
                    TimeSlots.Date, 
                    SUM(case when TimeSlotBookings.AttendanceCanceled = 0 then 1 else 0 end) as OccupiedSpaceCount
                FROM TimeSlots
                LEFT JOIN TimeSlotBookings
                ON TimeSlots.TimeSlotID = TimeSlotBookings.TimeSlotID
                GROUP BY TimeSlots.SourceID, TimeSlots.Date");

            migrationBuilder.Sql(@"CREATE VIEW DateOverview AS
                SELECT 
	                DateCapacity.SourceID,
	                DateCapacity.Date, 
	                DateCapacity.Capacity, 
	                DateOccupancy.OccupiedSpaceCount
                FROM DateOccupancy
                INNER JOIN DateCapacity ON DateOccupancy.Date = DateCapacity.Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ToExpected",
                table: "TimeSlotBookings",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FromExpected",
                table: "TimeSlotBookings",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
