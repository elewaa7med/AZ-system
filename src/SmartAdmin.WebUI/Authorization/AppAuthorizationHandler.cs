using Microsoft.AspNetCore.Authorization;
using SmartAdmin.WebUI.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Authorization
{
    public class AppAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        private readonly ApplicationDbContext _context;
        public AppAuthorizationHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            if (context.User == null)
                return Task.CompletedTask;

            var userPermission = context.User.FindFirstValue(requirement.Permission.ToString());
            if (userPermission == null)
                return Task.CompletedTask;

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
    public class OperationAuthorizationRequirement : IAuthorizationRequirement
    {
        public Permission Permission { get; set; }
    }

    public enum Permission
    {
        [Display(Name = "Create Contract")]
        [System.ComponentModel.DisplayName("Contract")]
        CreateContract = 1,

        [Display(Name = "Edit Contract")]
        [System.ComponentModel.DisplayName("Contract")]
        EditContract = 2,

        [Display(Name = "Delete Contract")]
        [System.ComponentModel.DisplayName("Contract")]
        DeleteContract = 3,

        [Display(Name = "Renew Contract")]
        [System.ComponentModel.DisplayName("Contract")]
        RenewContract = 4,

        [Display(Name = "Archive Contract")]
        [System.ComponentModel.DisplayName("Contract")]
        ArchiveContract = 5,

        [Display(Name = "Show Payment")]
        [System.ComponentModel.DisplayName("Contract")]
        ShowPayment = 6,

        [Display(Name = "Write Payment Notes")]
        [System.ComponentModel.DisplayName("Contract")]
        WritePaymentNote = 7,

        [Display(Name = "Make Payment")]
        [System.ComponentModel.DisplayName("Contract")]
        MakePayment = 8,

        [Display(Name = "Make Partial Payment")]
        [System.ComponentModel.DisplayName("Contract")]
        MakePartialPayment = 9,

        [Display(Name = "Undo Last Payment")]
        [System.ComponentModel.DisplayName("Contract")]
        UndoLastPayment = 10,

        [Display(Name = "Undo All Payments")]
        [System.ComponentModel.DisplayName("Contract")]
        UndoAllPayments = 11,

        

        [Display(Name = "Building")]
        [System.ComponentModel.DisplayName("Building")]
        BuildingAccountant = 12,

        [Display(Name = "Building srooh")]
        [System.ComponentModel.DisplayName("Building")]
        BuildingSroohAccountant = 33,

        [Display(Name = "Building Hyam Alrashed")]
        [System.ComponentModel.DisplayName("Building")]
        BuildingHyamAlrashed = 34,

        [Display(Name = "Building UK")]
        [System.ComponentModel.DisplayName("Building")]
        BuildingUK = 37,

        [Display(Name = "Desert Rose")]
        [System.ComponentModel.DisplayName("Compounds")]
        DesertRose = 13,

        [Display(Name = "Daar Residence")]
        [System.ComponentModel.DisplayName("Compounds")]
        DaarResidence = 14,

        [Display(Name = "Meadow Park Garden")]
        [System.ComponentModel.DisplayName("Compounds")]
        MeadowParkGarden = 15,

        [Display(Name = "24 Villa")]
        [System.ComponentModel.DisplayName("Compounds")]
        Villa24 = 16,

        [Display(Name = "Opal Compound")]
        [System.ComponentModel.DisplayName("Compounds")]
        Villa21 = 17,

        [Display(Name = "Legal")]
        [System.ComponentModel.DisplayName("Other")]
        Legal = 18,

        [Display(Name = "Legal Srooh")]
        [System.ComponentModel.DisplayName("Other")]
        LegalSrooh = 35,

        [Display(Name = "Legal UK")]
        [System.ComponentModel.DisplayName("Other")]
        LegalUK = 38,

        [Display(Name = "Legal Hyam Alrashed")]
        [System.ComponentModel.DisplayName("Other")]
        LegalHyamAlrashed = 36,


        [Display(Name = "Legal Daar Residence")]
        [System.ComponentModel.DisplayName("Other")]
        LegalDaarResidence = 42,

        [Display(Name = "Legal Desert Rose")]
        [System.ComponentModel.DisplayName("Other")]
        LegalDesertRose = 43,

        [Display(Name = "Legal Meadow Park")]
        [System.ComponentModel.DisplayName("Other")]
        LegalMeadowPark = 44,

        [Display(Name = "Legal Desert Apartments")]
        [System.ComponentModel.DisplayName("Other")]
        LegalDesertApartments = 45,

        [Display(Name = "Legal Villa24")]
        [System.ComponentModel.DisplayName("Other")]
        LegalVilla24 = 46,

        [Display(Name = "Legal Opal Compound")]
        [System.ComponentModel.DisplayName("Other")]
        LegalOpalCompound = 47,


        [Display(Name = "Delete Note")]
        [System.ComponentModel.DisplayName("Contract")]
        DeleteNote = 19,

        [Display(Name = "Building")]
        [System.ComponentModel.DisplayName("Dashboard")]
        BuildingDashboard = 20,

        [Display(Name = "Building Srooh")]
        [System.ComponentModel.DisplayName("Dashboard")]
        BuildingSroohDashboard = 39,

        [Display(Name = "Building Hayam elrashed")]
        [System.ComponentModel.DisplayName("Dashboard")]
        BuildingHayamElrashedDashboard = 40,

        [Display(Name = "Building Hayam elrashed")]
        [System.ComponentModel.DisplayName("Dashboard")]
        BuildingUKDashboard = 41,

        [Display(Name = "Daar Residence")]
        [System.ComponentModel.DisplayName("Dashboard")]
        DaarResidenceDashboard = 21,

        [Display(Name = "Desert Rose")]
        [System.ComponentModel.DisplayName("Dashboard")]
        DesertRoseDashboard = 22,

        [Display(Name = "24 Villa")]
        [System.ComponentModel.DisplayName("Dashboard")]
        Villa24Dashboard = 23,

        [Display(Name = "Opal Compound")]
        [System.ComponentModel.DisplayName("Dashboard")]
        Villa21Dashboard = 24,

        [Display(Name = "Meadow Park Garden")]
        [System.ComponentModel.DisplayName("Dashboard")]
        MeadowParkGardenDashboard = 25,

        [Display(Name = "Desert Apartments")]
        [System.ComponentModel.DisplayName("Compounds")]
        DesertApartments = 26,

        [Display(Name = "Add Compounds Buildings")]
        [System.ComponentModel.DisplayName("Compounds")]
        AddBuilding =28,

        [Display(Name = "Add Compounds Unites")]
        [System.ComponentModel.DisplayName("Compounds")]
        AddUnites = 30,

        [Display(Name = "Desert Apartments")]
        [System.ComponentModel.DisplayName("Dashboard")]
        DesertApartmentsDashboard = 27
    }
}
