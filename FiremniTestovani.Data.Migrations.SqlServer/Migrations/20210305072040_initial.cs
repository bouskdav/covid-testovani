using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations.SqlServer.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    SourceID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    URL = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    CompanyStreet = table.Column<string>(nullable: true),
                    CompanyZIP = table.Column<string>(nullable: true),
                    CompanyCity = table.Column<string>(nullable: true),
                    TestDuration = table.Column<double>(nullable: false),
                    AllowSlotCancelation = table.Column<bool>(nullable: false),
                    RequireSlotConfirmation = table.Column<bool>(nullable: false),
                    DefaultCulture = table.Column<string>(nullable: true),
                    EarliestNextPossibleTest = table.Column<int>(nullable: false),
                    EarliestNextPossibleTestNumericValue = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.SourceID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    SourceID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Sources_SourceID",
                        column: x => x.SourceID,
                        principalTable: "Sources",
                        principalColumn: "SourceID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    TimeSlotID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
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
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlotBookings",
                columns: table => new
                {
                    TimeSlotBookingID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeSlotID = table.Column<int>(nullable: false),
                    SourceID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    EmployeeID = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    SysAdditionalData = table.Column<string>(nullable: true),
                    FromExpected = table.Column<DateTime>(nullable: true),
                    ToExpected = table.Column<DateTime>(nullable: true),
                    FromActual = table.Column<DateTime>(nullable: true),
                    ToActual = table.Column<DateTime>(nullable: true),
                    AttendanceCanceled = table.Column<bool>(nullable: false),
                    AttendanceConfirmed = table.Column<bool>(nullable: false),
                    TestCompleted = table.Column<bool>(nullable: false),
                    TestResult = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlotBookings", x => x.TimeSlotBookingID);
                    table.ForeignKey(
                        name: "FK_TimeSlotBookings_Sources_SourceID",
                        column: x => x.SourceID,
                        principalTable: "Sources",
                        principalColumn: "SourceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeSlotBookings_TimeSlots_TimeSlotID",
                        column: x => x.TimeSlotID,
                        principalTable: "TimeSlots",
                        principalColumn: "TimeSlotID",
                        //onDelete: ReferentialAction.Cascade);
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SourceID",
                table: "AspNetUsers",
                column: "SourceID");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlotBookings_SourceID",
                table: "TimeSlotBookings",
                column: "SourceID");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlotBookings_TimeSlotID",
                table: "TimeSlotBookings",
                column: "TimeSlotID");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_SourceID",
                table: "TimeSlots",
                column: "SourceID");

            migrationBuilder.Sql(@"CREATE VIEW DateCapacity AS
                SELECT 
	                timeslots.SourceID,
	                timeslots.Date, 
	                SUM(Capacity) as Capacity
                FROM timeslots
                GROUP BY timeslots.SourceID, timeslots.Date");

            migrationBuilder.Sql(@"CREATE VIEW DateOccupancy AS
                SELECT 
	                timeslots.SourceID,
                    timeslots.Date, 
                    SUM(case when timeslotbookings.AttendanceCanceled = 0 then 1 else 0 end) as OccupiedSpaceCount
                FROM timeslots
                LEFT JOIN timeslotbookings
                ON timeslots.TimeSlotID = timeslotbookings.TimeSlotID
                GROUP BY timeslots.SourceID, timeslots.Date");

            migrationBuilder.Sql(@"CREATE VIEW DateOverview AS
                SELECT 
	                datecapacity.SourceID,
	                datecapacity.Date, 
	                datecapacity.Capacity, 
	                dateoccupancy.OccupiedSpaceCount
                FROM dateoccupancy
                INNER JOIN datecapacity ON dateoccupancy.Date = datecapacity.Date");
            
            migrationBuilder.Sql(@"CREATE VIEW TimeSlotOccupancy AS
                SELECT 
                    [TimeSlots].[SourceID],
	                [TimeSlots].[TimeSlotID],
	                [TimeSlots].[From],
	                [TimeSlots].[To],
	                [TimeSlots].[Capacity],
	                SUM(case when [TimeSlotBookings].[AttendanceCanceled] = 0 then 1 else 0 end) as OccupiedSpaceCount
                FROM TimeSlots
                LEFT JOIN TimeSlotBookings
                ON [TimeSlots].[TimeSlotID] = [TimeSlotBookings].[TimeSlotID]
                GROUP BY 
                [TimeSlots].[SourceID],
                [TimeSlots].[TimeSlotID],
                [TimeSlots].[From],
                [TimeSlots].[To],
                [TimeSlots].[Capacity]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "TimeSlotBookings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropTable(
                name: "Sources");
        }
    }
}
