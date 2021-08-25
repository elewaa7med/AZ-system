using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddContractFileAndFlatContractFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FlatContractFile",
                table: "TEmployees",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkContractFile",
                table: "TEmployees",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlatContractFile",
                table: "TEmployees");

            migrationBuilder.DropColumn(
                name: "WorkContractFile",
                table: "TEmployees");
        }
    }
}
