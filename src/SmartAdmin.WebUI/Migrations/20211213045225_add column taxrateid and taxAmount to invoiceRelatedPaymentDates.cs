using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class addcolumntaxrateidandtaxAmounttoinvoiceRelatedPaymentDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "InvoiceRelatedPaymentDates",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxRateId",
                table: "InvoiceRelatedPaymentDates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceRelatedPaymentDates_TaxRateId",
                table: "InvoiceRelatedPaymentDates",
                column: "TaxRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceRelatedPaymentDates_TaxRate_TaxRateId",
                table: "InvoiceRelatedPaymentDates",
                column: "TaxRateId",
                principalTable: "TaxRate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceRelatedPaymentDates_TaxRate_TaxRateId",
                table: "InvoiceRelatedPaymentDates");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceRelatedPaymentDates_TaxRateId",
                table: "InvoiceRelatedPaymentDates");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "InvoiceRelatedPaymentDates");

            migrationBuilder.DropColumn(
                name: "TaxRateId",
                table: "InvoiceRelatedPaymentDates");
        }
    }
}
