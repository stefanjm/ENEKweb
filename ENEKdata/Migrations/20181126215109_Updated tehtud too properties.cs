using Microsoft.EntityFrameworkCore.Migrations;

namespace ENEKdata.Migrations
{
    public partial class Updatedtehtudtooproperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "TehtudTood",
                newName: "BuildingType");

            migrationBuilder.AddColumn<int>(
                name: "YearDone",
                table: "TehtudTood",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearDone",
                table: "TehtudTood");

            migrationBuilder.RenameColumn(
                name: "BuildingType",
                table: "TehtudTood",
                newName: "Description");
        }
    }
}
