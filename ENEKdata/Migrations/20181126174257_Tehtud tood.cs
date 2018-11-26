using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ENEKdata.Migrations
{
    public partial class Tehtudtood : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TehtudTood",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TehtudTood", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TehtudTooImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImageFileName = table.Column<string>(nullable: false),
                    TehtudtooId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TehtudTooImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TehtudTooImages_TehtudTood_TehtudtooId",
                        column: x => x.TehtudtooId,
                        principalTable: "TehtudTood",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TehtudTooImages_TehtudtooId",
                table: "TehtudTooImages",
                column: "TehtudtooId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TehtudTooImages");

            migrationBuilder.DropTable(
                name: "TehtudTood");
        }
    }
}
