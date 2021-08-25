using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class PaymentsUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "TUnitRentContractPayments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "TUnitRentContractPayments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TUnitRentContractPayments_UserID",
                table: "TUnitRentContractPayments",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_TUnitRentContractPayments_AspNetUsers_UserID",
                table: "TUnitRentContractPayments",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TUnitRentContractPayments_AspNetUsers_UserID",
                table: "TUnitRentContractPayments");

            migrationBuilder.DropIndex(
                name: "IX_TUnitRentContractPayments_UserID",
                table: "TUnitRentContractPayments");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "TUnitRentContractPayments");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "TUnitRentContractPayments");
        }
    }
}
