using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddNotesToContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Units",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TUnitRentContract",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TCompoundUnits",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TCompoundUnits");
        }
    }
}
