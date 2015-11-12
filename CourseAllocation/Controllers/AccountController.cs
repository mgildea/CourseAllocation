using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using CourseAllocation.Models;
using System.Net.Mail;
using System.Net.Mime;
using CourseAllocation.Annotations;
using CourseAllocation.ViewModels;

namespace CourseAllocation.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                    if (user.EmailConfirmed)
                    {
                        await SignInAsync(user, model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Account activiation is awaiting email confirmation.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

    

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!user.EmailConfirmed)
                    {
                        return await SendAccountConfirmationEmail(user);
                    }
                    
                    await SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null || UserManager.FindById(userId) == null)
            {
                return View("Error");
            }

            IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            else
            {
                AddErrors(result);
                return View();
            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
                    return View();
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

   
       
        // GET: /Account/ResendConfirmEmail
        [AllowAnonymous]
        public ActionResult ResendConfirmEmail()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendConfirmEmail(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", string.Format("{0} does not exist.", model.Email));
                    return View();
                }

                if ((await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("", string.Format("{0} has already been confirmed.", user.Email));
                    return View();
                }

                return await SendAccountConfirmationEmail(user);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/ConfirmEmailSent
        [AllowAnonymous]
        public ActionResult ConfirmEmailSent()
        {
            return View();
        }


        protected async Task<ActionResult> SendAccountConfirmationEmail(ApplicationUser user)
        {
            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return RedirectToAction("ConfirmEmailSent", "Account");
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            if (code == null)
            {
                return View("Error");
            }
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "No user found.");
                    return View();
                }
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    AddErrors(result);
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpParamAction]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            /*ManageMessageId? message = null;*/
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                await SignInAsync(user, isPersistent: false);

                StatusMessage(Constant.AccountController.RemoveLoginSuccess, StatusMessageType.Success);
                //message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                StatusMessage(Constant.AccountController.Error, StatusMessageType.Danger);
                //message = ManageMessageId.Error;
            }

           // ViewBag.Message = message;
            return RedirectToAction("Manage"/*, new { Message = message }*/);
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(/*ManageMessageId? message*/)
        {
           /* ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";*/
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                        await SignInAsync(user, isPersistent: false);

                        StatusMessage(Constant.AccountController.ChangePasswordSuccess, StatusMessageType.Success);
                        return RedirectToAction("Manage"/*, new { Message = ManageMessageId.ChangePasswordSuccess }*/);
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        StatusMessage(Constant.AccountController.SetPasswordSuccess, StatusMessageType.Success);
                        return RedirectToAction("Manage"/*, new { Message = ManageMessageId.SetPasswordSuccess }*/);
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string loginProvider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(loginProvider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
                return RedirectToAction("ResendConfirmEmail");
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

     


        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpParamAction]
        public ActionResult LinkLogin(string loginProvider)
        {
            //if the user already has this provider linked, relogin using provider
            if(UserManager.GetLogins(User.Identity.GetUserId()).Select(m => m.LoginProvider).Contains(loginProvider))
                return ExternalLogin(loginProvider, Url.Action("Manage"));

            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(loginProvider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                    StatusMessage(Constant.AccountController.Error, StatusMessageType.Danger);
                
                return RedirectToAction("Manage"/*, new { Message = ManageMessageId.Error }*/);
            }
            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                StatusMessage(Constant.AccountController.LinkLoginSuccess, StatusMessageType.Success);
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage" /*, new { Message = ManageMessageId.Error }*/);
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {

                        if (!user.EmailConfirmed)
                        {
                            return await SendAccountConfirmationEmail(user); 
                        }

                        await SignInAsync(user, isPersistent: false);
                        
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //[ChildActionOnly]
        //public ActionResult RemoveAccountList()
        //{
        //    var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());

        //    ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;

        //    return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        //}


        [ChildActionOnly]
        public ActionResult ManageExternalLogins()
        {
            List<ExternalLoginViewModel> ExternalLogins = new List<ExternalLoginViewModel>();

            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            var AllowRemoval = HasPassword();

            foreach (var loginProvider in HttpContext.GetOwinContext().Authentication.GetExternalAuthenticationTypes())
            {
                ExternalLogins.Add(new ExternalLoginViewModel(){
                    LoginProvider = loginProvider,
                    Account = (AllowRemoval && linkedAccounts.Any()) ? linkedAccounts.DefaultIfEmpty(null).SingleOrDefault(m => m.LoginProvider == loginProvider.AuthenticationType) : null
                });
            }

            return (ActionResult)PartialView("_ManageExternalLoginsPartial", ExternalLogins.OrderBy(m => m.LoginProvider.AuthenticationType));
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private void SendEmail(string email, string callbackUrl, string subject, string message)
        {
            // For information on sending mail, please visit http://go.microsoft.com/fwlink/?LinkID=320771
            /*
            #region formatter
            string text = string.Format("Please click on this link to {0} {1}", callbackUrl, message);
            string html = string.Format("Please click on this <a href=\"{0}\">link</a> to {1}<br/>", callbackUrl, message);

            html += HttpUtility.HtmlEncode(@"Or copy the following link on the browser:" + message.Body);
            #endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(email);
            msg.To.Add(new MailAddress(email));
            msg.Subject = subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("joe@contoso.com", "XXXXXX");
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);
            */
        }

        

        /*
        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }*/

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}