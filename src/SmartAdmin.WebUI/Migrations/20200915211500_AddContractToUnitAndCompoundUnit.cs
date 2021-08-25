using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddContractToUnitAndCompoundUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitRentContractID",
                table: "Units",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitRentContractID",
                table: "TCompoundUnits",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_UnitRentContractID",
                table: "Units",
                column: "UnitRentContractID");

            migrationBuilder.CreateIndex(
                name: "IX_TCompoundUnits_UnitRentContractID",
                table: "TCompoundUnits",
                column: "UnitRentContractID");

            migrationBuilder.AddForeignKey(
                name: "FK_TCompoundUnits_TUnitRentContract_UnitRentContractID",
                table: "TCompoundUnits",
                column: "UnitRentContractID",
                principalTable: "TUnitRentContract",
                principalColumn: "IdRentContract",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_TUnitRentContract_UnitRentContractID",
                table: "Units",
                column: "UnitRentContractID",
                principalTable: "TUnitRentContract",
                principalColumn: "IdRentContract",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.Sql("delete from [dbo].[Units] where [IdRentContract] not in (select[IdRentContract] from [dbo].[TUnitRentContract])");
            migrationBuilder.Sql("delete from [dbo].[TCompoundUnits] where [IdRentContract] not in (select[IdRentContract] from [dbo].[TUnitRentContract])");
            migrationBuilder.Sql("Update Units set UnitRentContractID = IdRentContract where IdRentContract is not null");
            migrationBuilder.Sql("Update TCompoundUnits set UnitRentContractID = IdRentContract where IdRentContract is not null");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCompoundUnits_TUnitRentContract_UnitRentContractID",
                table: "TCompoundUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_TUnitRentContract_UnitRentContractID",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_UnitRentContractID",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_TCompoundUnits_UnitRentContractID",
                table: "TCompoundUnits");

            migrationBuilder.DropColumn(
                name: "UnitRentContractID",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "UnitRentContractID",
                table: "TCompoundUnits");
        }
    }
}
