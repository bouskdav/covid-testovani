using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations
{
    public partial class basicdatastructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceID",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    SourceID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    URL = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    CompanyStreet = table.Column<string>(nullable: true),
                    CompanyZIP = table.Column<string>(nullable: true),
                    CompanyCity = table.Column<string>(nullable: true),
                    TestDuration = table.Column<int>(nullable: false),
                    AllowSlotCancelation = table.Column<bool>(nullable: false),
                    RequireSlotConfirmation = table.Column<bool>(nullable: false),
                    DefaultCulture = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.SourceID);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    TimeSlotID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SourceID = table.Column<int>(nullable: false),
                    From = table.Column<DateTime>(nullable: false),
                    To = table.Column<DateTime>(nullable: false),
                    Capacity = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    Disabled = table.Column<bool>(nullable: false),
                    TestDuration = table.Column<int>(nullable: true),
                    AllowSlotCancelation = table.Column<bool>(nullable: true),
                    RequireSlotConfirmation = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.TimeSlotID);
                    table.ForeignKey(
                        name: "FK_TimeSlots_Sources_SourceID",
                        column: x => x.SourceID,
                        principalTable: "Sources",
                        principalColumn: "SourceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlotBookings",
                columns: table => new
                {
                    TimeSlotBookingID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TimeSlotID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    EmployeeID = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    SysAdditionalData = table.Column<string>(nullable: true),
                    FromExpected = table.Column<DateTime>(nullable: false),
                    ToExpected = table.Column<DateTime>(nullable: false),
                    FromActual = table.Column<DateTime>(nullable: true),
                    ToActual = table.Column<DateTime>(nullable: true),
                    AttendanceCanceled = table.Column<bool>(nullable: false),
                    AttendanceConfirmed = table.Column<bool>(nullable: false),
                    TestCompleted = table.Column<bool>(nullable: false),
                    TestResult = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlotBookings", x => x.TimeSlotBookingID);
                    table.ForeignKey(
                        name: "FK_TimeSlotBookings_TimeSlots_TimeSlotID",
                        column: x => x.TimeSlotID,
                        principalTable: "TimeSlots",
                        principalColumn: "TimeSlotID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SourceID",
                table: "AspNetUsers",
                column: "SourceID");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlotBookings_TimeSlotID",
                table: "TimeSlotBookings",
                column: "TimeSlotID");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_SourceID",
                table: "TimeSlots",
                column: "SourceID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Sources_SourceID",
                table: "AspNetUsers",
                column: "SourceID",
                principalTable: "Sources",
                principalColumn: "SourceID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Sources_SourceID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TimeSlotBookings");

            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SourceID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SourceID",
                table: "AspNetUsers");
        }
    }
}
