using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddPaidByColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "UnitRentContractPaymentLogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitRentContractPaymentLogs_UserID",
                table: "UnitRentContractPaymentLogs",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitRentContractPaymentLogs_AspNetUsers_UserID",
                table: "UnitRentContractPaymentLogs",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitRentContractPaymentLogs_AspNetUsers_UserID",
                table: "UnitRentContractPaymentLogs");

            migrationBuilder.DropIndex(
                name: "IX_UnitRentContractPaymentLogs_UserID",
                table: "UnitRentContractPaymentLogs");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "UnitRentContractPaymentLogs");
        }
    }
}
