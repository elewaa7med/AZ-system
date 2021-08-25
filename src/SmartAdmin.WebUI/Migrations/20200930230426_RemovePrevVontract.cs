using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class RemovePrevVontract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TUnitRentContract_TUnitRentContract_PrevIdRentContract",
                table: "TUnitRentContract");

            migrationBuilder.DropIndex(
                name: "IX_Units_UnitRentContractID",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_TUnitRentContract_PrevIdRentContract",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "PrevIdRentContract",
                table: "TUnitRentContract");

            migrationBuilder.CreateIndex(
                name: "IX_Units_UnitRentContractID",
                table: "Units",
                column: "UnitRentContractID",
                unique: true,
                filter: "[UnitRentContractID] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Units_UnitRentContractID",
                table: "Units");

            migrationBuilder.AddColumn<int>(
                name: "PrevIdRentContract",
                table: "TUnitRentContract",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_UnitRentContractID",
                table: "Units",
                column: "UnitRentContractID");

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
    }
}
