using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Models;

namespace SmartAdmin.WebUI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<TPositions> TPosition
        {
            get;
            set;
        }

        public DbSet<Countries> TCountries
        {
            get;
            set;
        }

        public DbSet<PropertyTypes> TPropertyTypes
        {
            get;
            set;
        }

        public DbSet<BuildingFloors> TBuildingFloors
        {
            get;
            set;
        }

        public DbSet<Owners> TOwners
        {
            get;
            set;
        }

        public DbSet<Districts> TDistricts
        {
            get;
            set;
        }

        public DbSet<Cities> TCities
        {
            get;
            set;
        }

        public DbSet<Buildings> TBuildings
        {
            get;
            set;
        }

        public DbSet<Tenants> TTenants
        {
            get;
            set;
        }

        public DbSet<Companies> TCompanies
        {
            get;
            set;
        }

        public DbSet<Mandoobs> TMandoobs
        {
            get;
            set;
        }

        public DbSet<Units> Units
        {
            get;
            set;
        }

        public DbSet<UnitKeys> TUnitKeys
        {
            get;
            set;
        }

        public DbSet<Nationalities> TNationalities
        {
            get;
            set;
        }

        public DbSet<Compounds> TCompounds
        {
            get;
            set;
        }

        public DbSet<CompoundsUsers> TCompoundsUsers
        {
            get;
            set;
        }

        public DbSet<MetersElectricityInfo> TMeterElectricityInfo
        {
            get;
            set;
        }

        public DbSet<MeterWaterInfo> TMeterWaterInfo
        {
            get;
            set;
        }

        public DbSet<CompoundBuildings> TCompoundBuildings
        {
            get;
            set;
        }

        public DbSet<CompoundUnits> TCompoundUnits
        {
            get;
            set;
        }

        public DbSet<CompoundUnitKeys> TCompoundUnitKeys
        {
            get;
            set;
        }

        public DbSet<UnitRentContract> TUnitRentContract
        {
            get;
            set;
        }

        public DbSet<DBJobs> TDBJobs
        {
            get;
            set;
        }

        public DbSet<DBJobHistory> TDBJobHistory
        {
            get;
            set;
        }

        public DbSet<Employees> TEmployees
        {
            get;
            set;
        }

        public DbSet<WorkPlaces> TWorkplaces
        {
            get;
            set;
        }

        public DbSet<ApplicationRole> ApplicationRole
        {
            get;
            set;
        }
        public DbSet<UnitRentContractPayment> TUnitRentContractPayments { get; set; }
        public DbSet<UnitRentContractNote> TUnitRentContractNotes { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<Legal> Legals { get; set; }
        public DbSet<ElectricityMeter> ElectricityMeters { get; set; }
        public DbSet<UnitRentContractPaymentLog> UnitRentContractPaymentLogs { get; set; }
        public DbSet<UnitRentContractAllPaymentLogs> unitRentContractAllPaymentLogs { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<MasterBuildings> MasterBuildings { get; set; }
        public DbSet<Invoices> Invoices { get; set; }
        public DbSet<InvoiceRelatedPaymentDates> InvoiceRelatedPaymentDates { get; set; }
        public DbSet<UnitRentContractOtherPayment> unitRentContractOtherPayment { get; set; }
        public DbSet<TaxRate> TaxRate { get; set; }
        public DbSet<TaxToProperttypes> taxToProperttypes { get; set; }
        public DbSet<CompoundContracts> compoundContracts { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
