using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class createrelationbetweenUnitRentContractOtherPaymentandInvoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitRentContractOtherPaymentID",
                table: "Invoices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_UnitRentContractOtherPaymentID",
                table: "Invoices",
                column: "UnitRentContractOtherPaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_unitRentContractOtherPayment_UnitRentContractOtherPaymentID",
                table: "Invoices",
                column: "UnitRentContractOtherPaymentID",
                principalTable: "unitRentContractOtherPayment",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_unitRentContractOtherPayment_UnitRentContractOtherPaymentID",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_UnitRentContractOtherPaymentID",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UnitRentContractOtherPaymentID",
                table: "Invoices");
        }
    }
}
