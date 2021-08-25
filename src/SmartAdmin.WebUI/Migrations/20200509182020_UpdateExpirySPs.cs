using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class UpdateExpirySPs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER PROCEDURE [dbo].[getContractExpiryofBuildings]
As
select 
UnitNumber,tenantName , tenantMobile , RentRate  ,convert(varchar,dtLeaseEnd,111) as dtLeaseEnd,
datediff(dd,getdate(), dtleaseend ) as DaysToContractExpiry,
ROW_NUMBER() Over (Order by datediff(dd,getdate(), dtleaseend )) as SN,
BuildingName + ' - ' + DistrictName as Building,
TUnitRentContract.IdRentContract,
yearlyrent/(leasePeriodInMonthes / (case paymentMethod when 1 then 1 when 2 then 3 when 3 then 4 when 4 then 6 when 5 then 12 end) ) as installmentValue
from TUnitRentContract 
inner join Units on units.IdUnit= TUnitRentContract.IdUnit 
inner join TBuildings on units.IdBuilding=TBuildings.IdBuilding
inner join TDistricts on TBuildings.IdDistrict=TDistricts.IdDistrict
inner join TTenants on ttenants.IdTenant=TUnitRentContract.IdTenant 
 where TUnitRentContract.Archived = 0 and DATEDIFF(day, getdate(), dtLeaseEnd) <= 60 
order by DaysToContractExpiry;
");
            migrationBuilder.Sql(@"ALTER PROCEDURE [dbo].[getContractExpiryPerCompound]
@IdCompound as integer
As
select UnitNumber,tenantName , tenantMobile , yearlyRent  ,convert(varchar,dtLeaseEnd,111) as dtLeaseEnd,
datediff(dd,getdate(), dtleaseend ) as DaysToContractExpiry,
ROW_NUMBER() Over (Order by datediff(dd,getdate(), dtleaseend )) as SN,
contractNumber,insurance,compoundName as compoundName, TUnitRentContract.IdCompound as IdCompound, 
TUnitRentContract.IdRentContract,yearlyrent/(leasePeriodInMonthes / (case paymentMethod when 1 then 1 when 2 then 3 when 3 then 4 when 4 then 6 when 5 then 12 end) ) as installmentValue from TUnitRentContract 
inner join TCompoundUnits on TCompoundUnits.IdUnit= TUnitRentContract.IdUnitCompound 
inner join TCompoundBuildings on TCompoundUnits.IdBuilding=TCompoundBuildings.IdBuilding
inner join TTenants on ttenants.IdTenant=TUnitRentContract.IdTenant 
inner join TCompounds on TCompounds.IdCompound = TUnitRentContract.IdCompound
where TUnitRentContract.Archived = 0 and DATEDIFF(day, getdate(), dtLeaseEnd) <= 60  and isnull(TUnitRentContract.IdCompound,0) = @IdCompound
order by DaysToContractExpiry;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
