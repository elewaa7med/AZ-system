using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class VerifiedContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotVerifiedReason",
                table: "TUnitRentContract",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VerifiedFromGovernment",
                table: "TUnitRentContract",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotVerifiedReason",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "VerifiedFromGovernment",
                table: "TUnitRentContract");
        }
    }
}
