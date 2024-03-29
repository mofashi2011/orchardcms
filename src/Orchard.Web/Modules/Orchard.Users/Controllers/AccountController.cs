﻿using System;
using System.Diagnostics.CodeAnalysis;
using Orchard.Localization;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.ViewModels;
using Orchard.Security;
using Orchard.Users.Services;
using Orchard.Users.ViewModels;

namespace Orchard.Users.Controllers {
    [HandleError]
    public class AccountController : Controller {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;

        public AccountController(
            IAuthenticationService authenticationService, 
            IMembershipService membershipService,
            IUserService userService) {
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _userService = userService;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public ActionResult AccessDenied() {
            var returnUrl = Request.QueryString["ReturnUrl"];
            var currentUser = _authenticationService.GetAuthenticatedUser();

            if (currentUser == null) {
                Logger.Information("Access denied to anonymous request on {0}", returnUrl);
                return View("LogOn", new LogOnViewModel {Title = "Access Denied"});
            }

            //TODO: (erikpo) Add a setting for whether or not to log access denieds since these can fill up a database pretty fast from bots on a high traffic site
            Logger.Information("Access denied to user #{0} '{1}' on {2}", currentUser.Id, currentUser.UserName, returnUrl);

            return View(new BaseViewModel());
        }

        public ActionResult LogOn() {
            if (_authenticationService.GetAuthenticatedUser() != null)
                return Redirect("~/");

            return View("LogOn", new LogOnViewModel {Title = "Log On"});
        }

        [HttpPost]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public ActionResult LogOn(string userNameOrEmail, string password, bool rememberMe, string returnUrl) {
            var user = ValidateLogOn(userNameOrEmail, password);
            if (!ModelState.IsValid) {
                return View("LogOn", new LogOnViewModel {Title = "Log On"});
            }

            _authenticationService.SignIn(user, rememberMe);

            if (string.IsNullOrEmpty(returnUrl))
                return new RedirectResult("~/");

            return new RedirectResult(returnUrl);
        }

        public ActionResult LogOff(string returnUrl) {
            _authenticationService.SignOut();

            if (string.IsNullOrEmpty(returnUrl))
                return new RedirectResult("~/");

            return new RedirectResult(returnUrl);
        }

        int MinPasswordLength {
            get {
                return _membershipService.GetSettings().MinRequiredPasswordLength;
            }
        }

        public ActionResult Register() {
            ViewData["PasswordLength"] = MinPasswordLength;

            return View(new BaseViewModel());
        }

        [HttpPost]
        public ActionResult Register(string userName, string email, string password, string confirmPassword) {
            ViewData["PasswordLength"] = MinPasswordLength;

            if (ValidateRegistration(userName, email, password, confirmPassword)) {
                // Attempt to register the user
                var user = _membershipService.CreateUser(new CreateUserParams(userName, password, email, null, null, true));


                if (user != null) {
                    _authenticationService.SignIn(user, false /* createPersistentCookie */);
                    return Redirect("~/");
                }
                else {
                    ModelState.AddModelError("_FORM", T(ErrorCodeToString(/*createStatus*/MembershipCreateStatus.ProviderError)));
                }
            }

            // If we got this far, something failed, redisplay form
            return Register();
        }

        [Authorize]
        public ActionResult ChangePassword() {
            ViewData["PasswordLength"] = MinPasswordLength;

            return View(new BaseViewModel());
        }

        [Authorize]
        [HttpPost]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword) {
            ViewData["PasswordLength"] = MinPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword)) {
                return View();
            }

            try {
                var validated = _membershipService.ValidateUser(User.Identity.Name, currentPassword);

                if (validated != null) {
                    _membershipService.SetPassword(validated, newPassword);
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else {
                    ModelState.AddModelError("_FORM",
                                             T("The current password is incorrect or the new password is invalid."));
                    return ChangePassword();
                }
            }
            catch {
                ModelState.AddModelError("_FORM", T("The current password is incorrect or the new password is invalid."));
                return ChangePassword();
            }
        }

        public ActionResult ChangePasswordSuccess() {
            return View(new BaseViewModel());
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity) {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        #region Validation Methods

        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword) {
            if (String.IsNullOrEmpty(currentPassword)) {
                ModelState.AddModelError("currentPassword", T("You must specify a current password."));
            }
            if (newPassword == null || newPassword.Length < MinPasswordLength) {
                ModelState.AddModelError("newPassword", T("You must specify a new password of {0} or more characters.", MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal)) {
                ModelState.AddModelError("_FORM", T("The new password and confirmation password do not match."));
            }

            return ModelState.IsValid;
        }

        private IUser ValidateLogOn(string userNameOrEmail, string password) {
            bool validate = true;

            if (String.IsNullOrEmpty(userNameOrEmail)) {
                ModelState.AddModelError("userNameOrEmail", T("You must specify a username or e-mail."));
                validate = false;
            }
            if (String.IsNullOrEmpty(password)) {
                ModelState.AddModelError("password", T("You must specify a password."));
                validate = false;
            }

            if (!validate)
                return null;

            var user = _membershipService.ValidateUser(userNameOrEmail, password);
            if (user == null) {
                ModelState.AddModelError("_FORM", T("The username or e-mail or password provided is incorrect."));
            }

            return user;
        }

        private bool ValidateRegistration(string userName, string email, string password, string confirmPassword) {
            bool validate = true;

            if (String.IsNullOrEmpty(userName)) {
                ModelState.AddModelError("username", T("You must specify a username."));
                validate = false;
            }
            if (String.IsNullOrEmpty(email)) {
                ModelState.AddModelError("email", T("You must specify an email address."));
                validate = false;
            }

            if (!validate)
                return false;

            string userUnicityMessage = _userService.VerifyUserUnicity(userName, email);
            if (userUnicityMessage != null) {
                ModelState.AddModelError("userExists", T(userUnicityMessage));
            }
            if (password == null || password.Length < MinPasswordLength) {
                ModelState.AddModelError("password", T("You must specify a password of {0} or more characters.", MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal)) {
                ModelState.AddModelError("_FORM", T("The new password and confirmation password do not match."));
            }
            return ModelState.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus) {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus) {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }

    public interface IMembershipServiceShim {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    public class AccountMembershipService : IMembershipServiceShim {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null) { }

        public AccountMembershipService(MembershipProvider provider) {
            _provider = provider ?? Membership.Provider;
        }

        #region IMembershipService Members

        public int MinPasswordLength {
            get { return _provider.MinRequiredPasswordLength; }
        }

        public bool ValidateUser(string userName, string password) {
            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email) {
            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword) {
            var currentUser = _provider.GetUser(userName, true /* userIsOnline */);
            return currentUser.ChangePassword(oldPassword, newPassword);
        }

        #endregion
    }
}