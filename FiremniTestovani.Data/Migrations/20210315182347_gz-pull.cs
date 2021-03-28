using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations
{
    public partial class gzpull : Migration
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

            migrationBuilder.AddColumn<string>(
                name: "InternalURL",
                table: "Sources",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    NameLocalized = table.Column<string>(nullable: true),
                    ISOCode = table.Column<string>(nullable: true),
                    DefaultInsuraceID = table.Column<int>(nullable: true),
                    PersonalIdentificationNumberPattern = table.Column<string>(nullable: true),
                    IsImportant = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropColumn(
                name: "SecurityCode",
                table: "TimeSlotBookings");

            migrationBuilder.DropColumn(
                name: "ValidationCode",
                table: "TimeSlotBookings");

            migrationBuilder.DropColumn(
                name: "InternalURL",
                table: "Sources");
        }
    }
}
