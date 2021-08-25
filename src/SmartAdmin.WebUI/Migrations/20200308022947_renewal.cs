using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class renewal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Renewed",
                table: "TUnitRentContract");

            migrationBuilder.AddColumn<int>(
                name: "PrevIdRentContract",
                table: "TUnitRentContract",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TUnitRentContract_PrevIdRentContract",
                table: "TUnitRentContract",
                column: "PrevIdRentContract");

            migrationBuilder.AddForeignKey(
                name: "FK_TUnitRentContract_TUnitRentContract_PrevIdRentContract",
                table: "TUnitRentContract",
                column: "PrevIdRentContract",
                principalTable: "TUnitRentContract",
                principalColumn: "IdRentContract",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TUnitRentContract_TUnitRentContract_PrevIdRentContract",
                table: "TUnitRentContract");

            migrationBuilder.DropIndex(
                name: "IX_TUnitRentContract_PrevIdRentContract",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "PrevIdRentContract",
                table: "TUnitRentContract");

            migrationBuilder.AddColumn<bool>(
                name: "Renewed",
                table: "TUnitRentContract",
                nullable: false,
                defaultValue: false);
        }
    }
}
