using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class addstatuscolumntoInvoiceandRelatedPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "InvoiceRelatedPaymentDates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "InvoiceRelatedPaymentDates");
        }
    }
}
