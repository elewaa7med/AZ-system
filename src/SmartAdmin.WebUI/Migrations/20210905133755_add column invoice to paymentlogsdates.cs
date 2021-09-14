using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class addcolumninvoicetopaymentlogsdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceID",
                table: "unitRentContractAllPaymentLogs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_unitRentContractAllPaymentLogs_InvoiceID",
                table: "unitRentContractAllPaymentLogs",
                column: "InvoiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_unitRentContractAllPaymentLogs_Invoices_InvoiceID",
                table: "unitRentContractAllPaymentLogs",
                column: "InvoiceID",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_unitRentContractAllPaymentLogs_Invoices_InvoiceID",
                table: "unitRentContractAllPaymentLogs");

            migrationBuilder.DropIndex(
                name: "IX_unitRentContractAllPaymentLogs_InvoiceID",
                table: "unitRentContractAllPaymentLogs");

            migrationBuilder.DropColumn(
                name: "InvoiceID",
                table: "unitRentContractAllPaymentLogs");
        }
    }
}
