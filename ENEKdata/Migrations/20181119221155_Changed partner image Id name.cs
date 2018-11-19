using Microsoft.EntityFrameworkCore.Migrations;

namespace ENEKdata.Migrations
{
    public partial class ChangedpartnerimageIdname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerImages_Partners_PartnerImageId",
                table: "PartnerImages");

            migrationBuilder.RenameColumn(
                name: "PartnerImageId",
                table: "PartnerImages",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerImages_Partners_Id",
                table: "PartnerImages",
                column: "Id",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerImages_Partners_Id",
                table: "PartnerImages");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PartnerImages",
                newName: "PartnerImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerImages_Partners_PartnerImageId",
                table: "PartnerImages",
                column: "PartnerImageId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
