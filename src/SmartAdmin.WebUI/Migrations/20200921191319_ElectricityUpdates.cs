using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class ElectricityUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ElectricityNumber",
                table: "TUnitRentContract",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentNumber",
                table: "TUnitRentContract",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ElectricityMeters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectricityNumber",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "PaymentNumber",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "ElectricityMeters");
        }
    }
}
