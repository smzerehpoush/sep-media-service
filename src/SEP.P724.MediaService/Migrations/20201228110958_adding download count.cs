using Microsoft.EntityFrameworkCore.Migrations;

namespace SEP.P724.MediaService.Migrations
{
    public partial class addingdownloadcount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DownloadCount",
                table: "Medias",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadCount",
                table: "Medias");
        }
    }
}
