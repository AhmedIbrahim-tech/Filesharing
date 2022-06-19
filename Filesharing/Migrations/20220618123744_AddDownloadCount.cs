using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Filesharing.Migrations
{
    public partial class AddDownloadCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DownloadCount",
                table: "Uploads",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadCount",
                table: "Uploads");
        }
    }
}
