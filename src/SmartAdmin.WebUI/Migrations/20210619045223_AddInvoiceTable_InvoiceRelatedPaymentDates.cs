using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddInvoiceTable_InvoiceRelatedPaymentDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Payment = table.Column<decimal>(nullable: false),
                    ContractId = table.Column<int>(nullable: false),
                    unitRentContractIdRentContract = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_TUnitRentContract_unitRentContractIdRentContract",
                        column: x => x.unitRentContractIdRentContract,
                        principalTable: "TUnitRentContract",
                        principalColumn: "IdRentContract",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceRelatedPaymentDates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InvoiceId = table.Column<int>(nullable: false),
                    UnitRentContractPaymentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceRelatedPaymentDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceRelatedPaymentDates_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceRelatedPaymentDates_TUnitRentContractPayments_UnitRentContractPaymentId",
                        column: x => x.UnitRentContractPaymentId,
                        principalTable: "TUnitRentContractPayments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceRelatedPaymentDates_InvoiceId",
                table: "InvoiceRelatedPaymentDates",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceRelatedPaymentDates_UnitRentContractPaymentId",
                table: "InvoiceRelatedPaymentDates",
                column: "UnitRentContractPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_unitRentContractIdRentContract",
                table: "Invoices",
                column: "unitRentContractIdRentContract");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceRelatedPaymentDates");

            migrationBuilder.DropTable(
                name: "Invoices");
        }
    }
}
