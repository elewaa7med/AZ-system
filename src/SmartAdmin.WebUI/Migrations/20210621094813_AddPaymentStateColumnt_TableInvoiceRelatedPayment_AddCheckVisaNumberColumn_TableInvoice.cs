using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddPaymentStateColumnt_TableInvoiceRelatedPayment_AddCheckVisaNumberColumn_TableInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "checkVisaNumber",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PaymentState",
                table: "InvoiceRelatedPaymentDates",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkVisaNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaymentState",
                table: "InvoiceRelatedPaymentDates");
        }
    }
}
