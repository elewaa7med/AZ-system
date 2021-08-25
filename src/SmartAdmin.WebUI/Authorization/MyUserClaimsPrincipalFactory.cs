using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SmartAdmin.WebUI.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Authorization
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        private readonly Data.ApplicationDbContext _context;
        public MyUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor, Data.ApplicationDbContext context) : base(userManager, optionsAccessor)
        {
            _context = context;
        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var userRolesIDs = _context.UserRoles.Where(u => u.UserId == user.Id).Select(r => r.RoleId).ToList();
            var roles = _context.Roles.Where(ro => userRolesIDs.Contains(ro.Id)).Select(r => r.Name).ToList();
            foreach (var role in roles)
                identity.AddClaim(new Claim(ClaimTypes.Role, role));

            //var userRole = _context.Roles.FirstOrDefault(ro => ro.Id == _context.UserRoles.Where(u => u.UserId == user.Id).Select(r => r.RoleId).FirstOrDefault())?.Name ?? string.Empty;
            var permissions = _context.UserPermissions.Where(u => u.UserId == user.Id).Select(p => p.Permission);
            foreach (var permission in permissions)
                identity.AddClaim(new Claim(permission.ToString(), ((int)permission).ToString()));

            return identity;
        }
    }
}
