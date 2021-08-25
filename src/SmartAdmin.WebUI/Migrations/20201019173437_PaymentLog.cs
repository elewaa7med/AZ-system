using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class PaymentLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitRentContractPaymentLogs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaidAmount = table.Column<int>(nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    UnitRentContractPaymentID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitRentContractPaymentLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UnitRentContractPaymentLogs_TUnitRentContractPayments_UnitRentContractPaymentID",
                        column: x => x.UnitRentContractPaymentID,
                        principalTable: "TUnitRentContractPayments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitRentContractPaymentLogs_UnitRentContractPaymentID",
                table: "UnitRentContractPaymentLogs",
                column: "UnitRentContractPaymentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitRentContractPaymentLogs");
        }
    }
}
