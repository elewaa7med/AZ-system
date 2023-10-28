using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class updatetabletenantswithwhatsapp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Whatsapp",
                table: "TTenants",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Whatsapp",
                table: "TTenants");
        }
    }
}
