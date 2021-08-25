using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class updatecolumn_UnitRentContractAllPaymentLogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_unitRentContractAllPaymentLogs_TUnitRentContractPayments_UnitRentContractPaymentID",
                table: "unitRentContractAllPaymentLogs");

            migrationBuilder.RenameColumn(
                name: "UnitRentContractPaymentID",
                table: "unitRentContractAllPaymentLogs",
                newName: "UnitRentContractID");

            migrationBuilder.RenameColumn(
                name: "PaidAmount",
                table: "unitRentContractAllPaymentLogs",
                newName: "AllPaidAmount");

            migrationBuilder.RenameIndex(
                name: "IX_unitRentContractAllPaymentLogs_UnitRentContractPaymentID",
                table: "unitRentContractAllPaymentLogs",
                newName: "IX_unitRentContractAllPaymentLogs_UnitRentContractID");

            migrationBuilder.AddForeignKey(
                name: "FK_unitRentContractAllPaymentLogs_TUnitRentContract_UnitRentContractID",
                table: "unitRentContractAllPaymentLogs",
                column: "UnitRentContractID",
                principalTable: "TUnitRentContract",
                principalColumn: "IdRentContract",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_unitRentContractAllPaymentLogs_TUnitRentContract_UnitRentContractID",
                table: "unitRentContractAllPaymentLogs");

            migrationBuilder.RenameColumn(
                name: "UnitRentContractID",
                table: "unitRentContractAllPaymentLogs",
                newName: "UnitRentContractPaymentID");

            migrationBuilder.RenameColumn(
                name: "AllPaidAmount",
                table: "unitRentContractAllPaymentLogs",
                newName: "PaidAmount");

            migrationBuilder.RenameIndex(
                name: "IX_unitRentContractAllPaymentLogs_UnitRentContractID",
                table: "unitRentContractAllPaymentLogs",
                newName: "IX_unitRentContractAllPaymentLogs_UnitRentContractPaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_unitRentContractAllPaymentLogs_TUnitRentContractPayments_UnitRentContractPaymentID",
                table: "unitRentContractAllPaymentLogs",
                column: "UnitRentContractPaymentID",
                principalTable: "TUnitRentContractPayments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
