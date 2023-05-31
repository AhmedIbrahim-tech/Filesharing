using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Filesharing.Migrations
{
    public partial class FixnameContentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContantType",
                table: "Uploads",
                newName: "ContentType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "Uploads",
                newName: "ContantType");
        }
    }
}
