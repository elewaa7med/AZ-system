using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddNoteToPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "TUnitRentContractPayments",
                nullable: true);
//            migrationBuilder.Sql(@"
//Create  PROCEDURE [dbo].[UpdateLastPayment]
//@dtlastrentpayment as date,
//@IdContract as  integer
//As
//update TUnitRentContract set dtlastrentpayment= @dtlastrentpayment where IdRentContract=@IdContract;
//                                    ");
            migrationBuilder.Sql(@"ALTER  PROCEDURE [dbo].[setLastPayment]
@IdContract as  integer
As
declare @paymentMethod as integer;
declare @dtlastrentpayment as date,  @dtLeaseStart as date;


select @paymentMethod=paymentMethod, @dtlastrentpayment=dtlastrentpayment, @dtLeaseStart=dtLeaseStart
from TUnitRentContract where IdRentContract=@IdContract; 

select @dtlastrentpayment = 
dateadd(MM,(case @paymentMethod when 1 then 1 when 2 then 3 when 3 then 4 when 4 then 6 when 5 then 12 end), isnull(@dtlastrentpayment,@dtLeaseStart)) ;

update TUnitRentContract set dtlastrentpayment= @dtlastrentpayment where IdRentContract=@IdContract;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "TUnitRentContractPayments");
        }
    }
}
