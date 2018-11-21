using Microsoft.EntityFrameworkCore.Migrations;

namespace ENEKdata.Migrations
{
    public partial class changedpartnerimagekeyforeignkeyandcolumnname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerImages_Partners_Id",
                table: "PartnerImages");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PartnerImages",
                newName: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerImages_Partners_PartnerId",
                table: "PartnerImages",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerImages_Partners_PartnerId",
                table: "PartnerImages");

            migrationBuilder.RenameColumn(
                name: "PartnerId",
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
    }
}
