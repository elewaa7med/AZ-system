using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SmartAdmin.WebUI.Authorization;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Extensions;
using SmartAdmin.WebUI.Models;
using SmartAdmin.WebUI.Models.ManageViewModels;
using SmartAdmin.WebUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IEmailSender _emailSender;

        private readonly ILogger _logger;

        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private const string RecoveryCodesKey = "RecoveryCodesKey";

        [TempData]
        public string StatusMessage
        {
            get;
            set;
        }

        public ManageController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ILogger<ManageController> logger, UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            IndexViewModel model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!base.ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            string email = user.Email;
            if (model.Email != email && !(await _userManager.SetEmailAsync(user, model.Email)).Succeeded)
            {
                throw new ApplicationException("Unexpected error occurred setting email for user with ID '" + user.Id + "'.");
            }
            string phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber && !(await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber)).Succeeded)
            {
                throw new ApplicationException("Unexpected error occurred setting phone number for user with ID '" + user.Id + "'.");
            }
            StatusMessage = "Your profile has been updated";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!base.ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string callbackUrl = base.Url.EmailConfirmationLink(user.Id, code, base.Request.Scheme);
            string email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);
            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            if (!(await _userManager.HasPasswordAsync(user)))
            {
                return RedirectToAction("SetPassword");
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                StatusMessage = StatusMessage
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!base.ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            IdentityResult changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";
            return RedirectToAction("ChangePassword");
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            if (await _userManager.HasPasswordAsync(user))
            {
                return RedirectToAction("ChangePassword");
            }
            SetPasswordViewModel model = new SetPasswordViewModel
            {
                StatusMessage = StatusMessage
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!base.ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            IdentityResult addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";
            return RedirectToAction("SetPassword");
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            ExternalLoginsViewModel externalLoginsViewModel = new ExternalLoginsViewModel();
            ExternalLoginsViewModel externalLoginsViewModel2 = externalLoginsViewModel;
            externalLoginsViewModel2.CurrentLogins = await _userManager.GetLoginsAsync(user);
            ExternalLoginsViewModel model = externalLoginsViewModel;
            externalLoginsViewModel = model;
            externalLoginsViewModel.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Where((AuthenticationScheme auth) => model.CurrentLogins.All((UserLoginInfo ul) => auth.Name != ul.LoginProvider)).ToList();
            externalLoginsViewModel = model;
            externalLoginsViewModel.ShowRemoveButton = (await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1);
            model.StatusMessage = StatusMessage;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            await base.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            string redirectUrl = base.Url.Action("LinkLoginCallback");
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(base.User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException("Unexpected error occurred loading external login info for user with ID '" + user.Id + "'.");
            }
            if (!(await _userManager.AddLoginAsync(user, info)).Succeeded)
            {
                throw new ApplicationException("Unexpected error occurred adding external login for user with ID '" + user.Id + "'.");
            }
            await base.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            StatusMessage = "The external login was added.";
            return RedirectToAction("ExternalLogins");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            if (!(await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey)).Succeeded)
            {
                throw new ApplicationException("Unexpected error occurred removing external login for user with ID '" + user.Id + "'.");
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction("ExternalLogins");
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            TwoFactorAuthenticationViewModel twoFactorAuthenticationViewModel = new TwoFactorAuthenticationViewModel();
            TwoFactorAuthenticationViewModel twoFactorAuthenticationViewModel2 = twoFactorAuthenticationViewModel;
            twoFactorAuthenticationViewModel2.HasAuthenticator = (await _userManager.GetAuthenticatorKeyAsync(user) != null);
            twoFactorAuthenticationViewModel.Is2faEnabled = user.TwoFactorEnabled;
            TwoFactorAuthenticationViewModel twoFactorAuthenticationViewModel3 = twoFactorAuthenticationViewModel;
            twoFactorAuthenticationViewModel3.RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);
            return View(twoFactorAuthenticationViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException("Unexpected error occured disabling 2FA for user with ID '" + user.Id + "'.");
            }
            return View("Disable2fa");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            if (!(await _userManager.SetTwoFactorEnabledAsync(user, enabled: false)).Succeeded)
            {
                throw new ApplicationException("Unexpected error occured disabling 2FA for user with ID '" + user.Id + "'.");
            }
            _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction("TwoFactorAuthentication");
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            EnableAuthenticatorViewModel model = new EnableAuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            if (!base.ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }
            string verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            if (!(await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode)))
            {
                base.ModelState.AddModelError("Code", "Verification code is invalid.");
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }
            await _userManager.SetTwoFactorEnabledAsync(user, enabled: true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            base.TempData["RecoveryCodesKey"] = recoveryCodes.ToArray();
            return RedirectToAction("ShowRecoveryCodes");
        }

        [HttpGet]
        public IActionResult ShowRecoveryCodes()
        {
            string[] recoveryCodes = (string[])base.TempData["RecoveryCodesKey"];
            if (recoveryCodes == null)
            {
                return RedirectToAction("TwoFactorAuthentication");
            }
            ShowRecoveryCodesViewModel model = new ShowRecoveryCodesViewModel
            {
                RecoveryCodes = recoveryCodes
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View("ResetAuthenticator");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            await _userManager.SetTwoFactorEnabledAsync(user, enabled: false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);
            return RedirectToAction("EnableAuthenticator");
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodesWarning()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException("Cannot generate recovery codes for user with ID '" + user.Id + "' because they do not have 2FA enabled.");
            }
            return View("GenerateRecoveryCodes");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            ApplicationUser user = await _userManager.GetUserAsync(base.User);
            if (user == null)
            {
                throw new ApplicationException("Unable to load user with ID '" + _userManager.GetUserId(base.User) + "'.");
            }
            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException("Cannot generate recovery codes for user with ID '" + user.Id + "' as they do not have 2FA enabled.");
            }
            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);
            ShowRecoveryCodesViewModel model = new ShowRecoveryCodesViewModel
            {
                RecoveryCodes = recoveryCodes.ToArray()
            };
            return View("ShowRecoveryCodes", model);
        }

        public IActionResult ListAppUsers()
        {
            IOrderedQueryable<ApplicationUser> users = _userManager.Users.OrderBy((ApplicationUser m) => m.fullName);
            return View("ListAppUsers", users);
        }

        public async Task<IActionResult> DeActivateUser(string Id)
        {
            ApplicationUser _usr = await _userManager.FindByIdAsync(Id);
            if (_usr == null)
            {
                return NotFound();
            }
            return View(_usr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeActivateUserApply(string Id)
        {
            ApplicationUser _usr = await _userManager.FindByIdAsync(Id);
            try
            {
                if (!(await _userManager.IsLockedOutAsync(_usr)))
                {
                    await _userManager.SetLockoutEnabledAsync(_usr, enabled: true);
                    await _userManager.SetLockoutEndDateAsync(_usr, DateTime.Parse("1-1-2050"));
                }
                else if ((await _userManager.SetLockoutEnabledAsync(_usr, enabled: false)).Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(_usr);
                }
            }
            catch (Exception err)
            {
                base.ViewData["DeleteError"] = err.InnerException.Message;
                return View(_usr);
            }
            _userManager.Users.OrderBy((ApplicationUser m) => m.fullName);
            return RedirectToAction("ListAppUsers");
        }

        public async Task<IActionResult> DeleteUser(string Id)
        {
            ApplicationUser _usr = await _userManager.FindByIdAsync(Id);
            ViewBag.Users = new SelectList(_context.Users.Where(u => u.Id != Id), "Id", "fullName");
            if (_usr == null)
                return NotFound();

            return View(_usr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserApply(string Id, string replaceId)
        {
            ApplicationUser _usr = await _userManager.FindByIdAsync(Id);
            try
            {
                if (_usr == null)
                {
                    base.ViewData["AlertSaveErr"] = "User Does Not Exist";
                    return View("DeleteUser", _usr);
                }
                if (!string.IsNullOrEmpty(replaceId))
                {
                    foreach (var contract in _context.TUnitRentContract.Where(c => c.IdCreated == Id || c.IdMandoob == Id || c.IdModified == Id))
                    {
                        contract.IdMandoob = contract.IdMandoob == Id ? replaceId : contract.IdMandoob;
                        contract.IdCreated = contract.IdCreated == Id ? replaceId : contract.IdCreated;
                        contract.IdModified = contract.IdModified == Id ? replaceId : contract.IdModified;
                    }
                    foreach (var building in _context.TBuildings.Where(b => b.IdCreatedBy == Id || b.IdModifiedBy == Id))
                    {
                        building.IdCreatedBy = building.IdCreatedBy == Id ? replaceId : building.IdCreatedBy;
                        building.IdModifiedBy = building.IdModifiedBy == Id ? replaceId : building.IdModifiedBy;
                    }
                    foreach (var building in _context.TCompoundBuildings.Where(b => b.IdCreatedBy == Id || b.IdModifiedBy == Id))
                    {
                        building.IdCreatedBy = building.IdCreatedBy == Id ? replaceId : building.IdCreatedBy;
                        building.IdModifiedBy = building.IdModifiedBy == Id ? replaceId : building.IdModifiedBy;
                    }
                    foreach (var compound in _context.TCompounds.Where(b => b.IdCreated == Id || b.IdModified == Id))
                    {
                        compound.IdCreated = compound.IdCreated == Id ? replaceId : compound.IdCreated;
                        compound.IdModified = compound.IdModified == Id ? replaceId : compound.IdModified;
                    }
                    foreach (var compoundUser in _context.TCompoundsUsers.Where(u => u.IdUser == Id).ToList())
                        _context.TCompoundsUsers.Remove(compoundUser);
                    foreach (var compoundUnit in _context.TCompoundUnits.Where(u => u.IdCreatedBy == Id || u.IdModifiedBy == Id))
                    {
                        compoundUnit.IdCreatedBy = compoundUnit.IdCreatedBy == Id ? replaceId : compoundUnit.IdCreatedBy;
                        compoundUnit.IdModifiedBy = compoundUnit.IdModifiedBy == Id ? replaceId : compoundUnit.IdModifiedBy;
                    }
                    foreach (var employee in _context.TEmployees.Where(e => e.IdCreated == Id || e.IdModified == Id))
                    {
                        employee.IdCreated = employee.IdCreated == Id ? replaceId : employee.IdCreated;
                        employee.IdModified = employee.IdModified == Id ? replaceId : employee.IdModified;
                    }
                    foreach (var owner in _context.TOwners.Where(e => e.IdCreatedBy == Id || e.IdModifiedBy == Id))
                    {
                        owner.IdCreatedBy = owner.IdCreatedBy == Id ? replaceId : owner.IdCreatedBy;
                        owner.IdModifiedBy = owner.IdModifiedBy == Id ? replaceId : owner.IdModifiedBy;
                    }
                    foreach (var tenant in _context.TTenants.Where(e => e.IdCreatedBy == Id || e.IdModifiedBy == Id))
                    {
                        tenant.IdCreatedBy = tenant.IdCreatedBy == Id ? replaceId : tenant.IdCreatedBy;
                        tenant.IdModifiedBy = tenant.IdModifiedBy == Id ? replaceId : tenant.IdModifiedBy;
                    }
                    foreach (var unit in _context.Units.Where(e => e.IdCreatedBy == Id || e.IdModifiedBy == Id))
                    {
                        unit.IdCreatedBy = unit.IdCreatedBy == Id ? replaceId : unit.IdCreatedBy;
                        unit.IdModifiedBy = unit.IdModifiedBy == Id ? replaceId : unit.IdModifiedBy;
                    }
                    foreach (var note in _context.TUnitRentContractNotes.Where(e => e.UserID == Id))
                        note.UserID = replaceId;

                    foreach (var log in _context.UnitRentContractPaymentLogs.Where(e => e.UserID == Id))
                        log.UserID = replaceId;

                    foreach (var payment in _context.TUnitRentContractPayments.Where(e => e.UserID == Id))
                        payment.UserID = replaceId;
                    foreach (var AllPaymentLogs in _context.unitRentContractAllPaymentLogs.Where(e => e.UserID == Id))
                        AllPaymentLogs.UserID = replaceId; 
                    foreach (var Maintenance in _context.Maintenances.Where(e => e.UserID == Id))
                        Maintenance.UserID = replaceId;
                }
                _context.UserPermissions.RemoveRange(_context.UserPermissions.Where(u => u.UserId == Id).ToList());
                _context.SaveChanges();
                await _userManager.DeleteAsync(_usr);
            }
            catch (Exception err)
            {
                base.ViewData["AlertSaveErr"] = err.InnerException.Message;
                ViewBag.Users = new SelectList(_context.Users.Where(u => u.Id != Id), "Id", "fullName", replaceId);

                return View("DeleteUser", _usr);
            }
            return RedirectToAction("ListAppUsers");
        }

        public async Task<IActionResult> EditUser(string Id)
        {
            ApplicationUser _usr = await _userManager.FindByIdAsync(Id);
            if (_usr == null)
            {
                return NotFound();
            }
            return View(_usr);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(ApplicationUser updusr)
        {
            ApplicationUser _usr = await _userManager.FindByIdAsync(updusr.Id);
            if (_usr == null)
            {
                return NotFound();
            }
            _usr.Email = updusr.Email;
            _usr.fullName = updusr.fullName;
            _usr.UserName = updusr.UserName;
            IdentityResult result = await _userManager.UpdateAsync(_usr);
            if (result.Succeeded)
            {
                return RedirectToAction("ListAppUsers");
            }
            base.ViewBag.AlertSaveErr = "Error" + result.Errors.Last().ToString();
            return View(_usr);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                base.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            StringBuilder result = new StringBuilder();
            int currentPosition;
            for (currentPosition = 0; currentPosition + 4 < unformattedKey.Length; currentPosition += 4)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }
            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format("otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6", _urlEncoder.Encode("AZBMS"), _urlEncoder.Encode(email), unformattedKey);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, EnableAuthenticatorViewModel model)
        {
            string unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }
        public IActionResult UserPermissions(string userID)
        {
            var currentPermissions = _context.UserPermissions.Where(u => u.UserId == userID).Select(u => u.Permission).ToList();
            ViewBag.Permissions = Enum.GetValues(typeof(Permission)).Cast<Permission>().Select(v => new SelectListItem
            {
                Text = v.GetDisplayName(),
                Value = ((int)v).ToString(),
                Selected = currentPermissions.Contains(v),
                Group = new SelectListGroup
                {
                    Name = v.GetGroupName()
                }
            }).ToList();
            return PartialView("~/Views/Manage/_UserPermissions.cshtml", new UserPermissionDTO { UserID = userID });
        }
        [HttpPost]
        public IActionResult UserPermissions(UserPermissionDTO dto)
        {
            var currentPermissions = _context.UserPermissions.Where(u => u.UserId == dto.UserID);
            _context.UserPermissions.RemoveRange(currentPermissions.ToList());
            if (dto.Permissions != null)
                _context.UserPermissions.AddRange(dto.Permissions.Select(p => new UserPermission { UserId = dto.UserID, Permission = (Permission)p }));
            _context.SaveChanges();
            return RedirectToAction("ListAppUsers");
        }
        public void SeedPermission()
        {
            var permissions = Enum.GetValues(typeof(Permission)).Cast<Permission>().Select(v => v).ToList();
            foreach (var user in _context.Users)
                foreach (var permission in permissions)
                    user.UserPermissions.Add(new UserPermission
                    {
                        Permission = permission
                    });
            _context.SaveChanges();
        }
    }
    public class UserPermissionDTO
    {
        public string UserID { get; set; }
        public List<int> Permissions { get; set; }
    }
}
