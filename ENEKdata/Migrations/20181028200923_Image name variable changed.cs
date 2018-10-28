using Microsoft.EntityFrameworkCore.Migrations;

namespace ENEKdata.Migrations
{
    public partial class Imagenamevariablechanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Images",
                newName: "ImageFileName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageFileName",
                table: "Images",
                newName: "ImagePath");
        }
    }
}
