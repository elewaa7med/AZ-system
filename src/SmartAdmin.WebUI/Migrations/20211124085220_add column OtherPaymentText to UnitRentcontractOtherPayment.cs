using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class addcolumnOtherPaymentTexttoUnitRentcontractOtherPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherPaymentText",
                table: "unitRentContractOtherPayment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherPaymentText",
                table: "unitRentContractOtherPayment");

            
        }
    }
}
