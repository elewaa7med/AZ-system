using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddArabicDesc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionAR",
                table: "Units",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotesAR",
                table: "TUnitRentContract",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAR",
                table: "TCompoundUnits",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAR",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "NotesAR",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "DescriptionAR",
                table: "TCompoundUnits");
        }
    }
}
