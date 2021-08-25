using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddUnitOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitOwner",
                table: "Units",
                nullable: false,
                defaultValue:1);

            migrationBuilder.AddColumn<int>(
                name: "UnitOwner",
                table: "TCompoundUnits",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitOwner",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "UnitOwner",
                table: "TCompoundUnits");
        }
    }
}
