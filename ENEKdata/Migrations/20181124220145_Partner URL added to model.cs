using Microsoft.EntityFrameworkCore.Migrations;

namespace ENEKdata.Migrations
{
    public partial class PartnerURLaddedtomodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PartnerURL",
                table: "Partners",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartnerURL",
                table: "Partners");
        }
    }
}
