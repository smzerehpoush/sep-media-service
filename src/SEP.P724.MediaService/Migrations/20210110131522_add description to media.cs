using Microsoft.EntityFrameworkCore.Migrations;

namespace SEP.P724.MediaService.Migrations
{
    public partial class adddescriptiontomedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Medias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Medias");
        }
    }
}
