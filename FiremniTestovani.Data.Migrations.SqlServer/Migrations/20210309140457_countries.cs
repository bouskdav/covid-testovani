using Microsoft.EntityFrameworkCore.Migrations;

namespace FiremniTestovani.Data.Migrations.SqlServer.Migrations
{
    public partial class countries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    NameLocalized = table.Column<string>(nullable: true),
                    ISOCode = table.Column<string>(nullable: true),
                    DefaultInsuraceID = table.Column<int>(nullable: true),
                    PersonalIdentificationNumberPattern = table.Column<string>(nullable: true)
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
        }
    }
}
