using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class updatecolumnisOtherPaymenttoacceptnull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "isOtherPayment",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "isOtherPayment",
                table: "Invoices",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
