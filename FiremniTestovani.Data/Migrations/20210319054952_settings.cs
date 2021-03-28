using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations
{
    public partial class settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientConfigDir",
                table: "Sources",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    SourceID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    ValueType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => new { x.SourceID, x.Name });
                    table.ForeignKey(
                        name: "FK_Settings_Sources_SourceID",
                        column: x => x.SourceID,
                        principalTable: "Sources",
                        principalColumn: "SourceID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropColumn(
                name: "ClientConfigDir",
                table: "Sources");
        }
    }
}
