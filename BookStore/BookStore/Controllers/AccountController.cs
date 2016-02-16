using BookStore.DAL;
using BookStore.Models;
using Microsoft.Web.WebPages.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using System.Transactions;
using DotNetOpenAuth.AspNet;
using System.Security.Cryptography;
using System.Text;
using System.Web.Helpers;

namespace BookStore.Controllers
{
    /*  
       *  NAME    : AccountController
       *  PURPOSE : The methods in this class handle Account information for the user.
     *              if the user wishes to login or register, the controller takes care
     *              of their action. It makes use of it won view model and passes data to 
     *              its views using its view Model.
       */

    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        /*
        *  METHOD      : Login
        *  DESCRIPTION : 
        *      This method does a GET request method from the ..Account/Login
        *      page.  
        *  PARAMETERS  :
        *      string returnUrl : The URL to return to once "login" is pressed
        *  RETURNS     :
        *      View()           : The webpage for a user to log-in
        */
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        private static BookstoreContext db = new BookstoreContext();

        /*
         *  METHOD      : Login
         *  DESCRIPTION : 
         *      This method does a POST request method from the ..Account/Login
         *      page. 
         *  PARAMETERS  :
         *      User user        : Every user-account created is stored into the DB. 
         *                         This value stores who is currently trying to log in.
         *      string returnUrl : The URL to return to once login has been accepted
         *  RETURNS     :
         *      View()           : The webpage for a user to log-in
         */
        [HttpPost]
        [ActionName("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginPost(LoginModel user, string returnUrl)
        {
       
           // Byte[] a = EncryptPassword(user.Password);
            //string tmppwd = System.Web.Helpers.Crypto.HashPassword(user.Password);
            
           User resultUser = db.User.Where(u => (u.Email == user.Email)).FirstOrDefault();

           bool verifyPassword = Crypto.VerifyHashedPassword(resultUser.pword, user.Password);
            if (!verifyPassword)
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "The Email or Password provided is incorrect.");
                return View(user);
            }
            //HttpContext.User= new System.Web.Security.RolePrincipal();
            FormsAuthentication.SetAuthCookie(resultUser.Email, false);
            var role = Roles.GetRolesForUser(user.Email);
            if (role == null)
            {
                System.Web.Security.Roles.AddUsersToRole(new String[] { user.Email }, resultUser.Securitylevel);
            }
            return RedirectToLocal(returnUrl);
        }

        /*
         *  METHOD      : LogOff
         *  DESCRIPTION : 
         *      This method does a POST request method to log the user out 
         *      and return them to the front page.
         *  PARAMETERS  : N/A
         *  RETURNS     :
         *      RedirectToAction("Index", "Home") : Sends the user back to 
         *                                          the Index/Home page
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Login", "Account");
        }

        /*
         *  METHOD      : Register
         *  DESCRIPTION : 
         *      This method does a GET request method to register the user 
         *      into the database if they have the correct Email ending.  
         *  PARAMETERS  : N/A
         *  RETURNS     :
         *      View()  : Sends the user back to the Index/Home page
         */
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /*
         *  METHOD      : Register
         *  DESCRIPTION : 
         *      This method does a POST request method to register the user 
         *      into the database.  
         *  PARAMETERS  : 
         *      RegisterModel model : Current database for registerred users.
         *  RETURNS     :
         *      View(model)  : Sends the user back to the Index/Home page if no errors.
         */
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName("Register")]
        public ActionResult RegisterPost(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user with details provided
                try
                {
                    //create an array of bytes we will use to store the encrypted password
                   // Byte[] hashedBytes = EncryptPassword(model.Password);
                    string tmppwd = Crypto.HashPassword(model.Password);
                    WebSecurity.CreateUserAndAccount(model.Email, tmppwd);
                    User user = db.User.Where(u => u.Email == model.Email).FirstOrDefault();

                    //set hashed password
                    if (SetSecurityLevel(model.Email) == "")
                   {
                       ModelState.AddModelError("", "You must be a student from the following institutions: Conestoga College, Wilfred Laurier University , University Of Waterloo");
                       return View(model);
                    }
                   else if(SetSecurityLevel(model.Email) != "admin")
                   {
                       user.Securitylevel = "general";
                   }               
                   else
                   {
                       user.Securitylevel = "admin";
                   }
                    user.pword = tmppwd;
                    db.SaveChanges();

                    var userId = WebSecurity.GetUserId(User.Identity.Name);                     // AUDIT
                    var userID = user.UserID;
                    //audit information
                   AuditController.AuditEntry(userId, userID, AuditController.ADD_USER);

                   WebSecurity.Login(model.Email, tmppwd);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //public static string DEcryptPassword(Byte[] password)
        //{
        //    //output each byte individually to our label
        //    string pas = "";
        //    foreach (Byte b in password)
        //    {
        //        pas += b.ToString();
        //    }
        //    return pas;
        //}

        /*
         *  METHOD      : Disassociate
         *  DESCRIPTION : 
         *      This method does a POST request method to dissociate the currently 
         *      logged in user if the same user logs on.
         *  PARAMETERS  :
         *      string provider       : Currently logged in user
         *      string providerUserID : His/her ID
         *  RETURNS     :
         *      RedirectToAction("Manage", new {Message = message}) : 
         *          Moves the user to the Manage web page
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        /*
         *  METHOD      : Manage
         *  DESCRIPTION : 
         *      This method does a GET request method to return the messages
         *      when the user tries to change/set their password (/managing account).
         *  PARAMETERS  :
         *      ManageMessageID message : Nullable "message", containing error message
         *  RETURNS     :
         *      View()  : Sends the user back to the Manage page
         */
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        /*
         *  METHOD      : Manage
         *  DESCRIPTION : 
         *      This method does a POST request method to manage/modify the user 
         *      within the database.  
         *  PARAMETERS  : 
         *      LocalPasswordModel model : The Model/class containing the database
         *                                 password information.
         *  RETURNS     :
         *      RedirectToAction(...)    : Sends user back to the Manage page
         *           OR
         *      View(model)              : Sends the user back to the Index/Home page
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            //check if the user already has a local account to be managed
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded = false ;
                    //check if chnage password worked
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, Crypto.HashPassword( model.OldPassword), Crypto.HashPassword(model.NewPassword));
                   
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        var userId = WebSecurity.GetUserId(User.Identity.Name);

                        if (model.OldPassword != model.NewPassword)
                        {
                           AuditController.AuditEntry(userId, userId, AuditController.MODIFY_USER, "Password", model.OldPassword, model.NewPassword);

                        }

                        //Byte[] hashedBytes = EncryptPassword(model.NewPassword);

                        ////WebSecurity.CreateUserAndAccount(model.Email, model.Password);
                        //User user = db.User.Where(u => u.Email == model.).FirstOrDefault();

                        ////set hashed password
                        //user.pword = hashedBytes;
                        //db.SaveChanges();

                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, Crypto.HashPassword( model.NewPassword));
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the Name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /*
         *  METHOD      : ExternalLogin
         *  DESCRIPTION : 
         *      This method does a POST request method to try and log user into account.
         *      Uses the "ExternalLoginCallBack" function to specifically find error
         *  PARAMETERS  : 
         *      string provider  : The user
         *      string returnUrl : Web page to return to
         *  RETURNS     :
         *      new ExternalLoginResult(...)  : Calls ExternalLoginCallback ActionResult
         */
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        /*
         *  METHOD      : ExternalLoginCallback
         *  DESCRIPTION : 
         *      This method does a GET request method to register the user 
         *      into the database.  
         *  PARAMETERS  :
         *      string returnUrl : Webpage to return to
         *  RETURNS     :
         *      RedirectToAction("ExternalLoginFailure") : Authentication failed, show fail page
         *          OR
         *      RedirectToLocal(returnUrl)               : head to returnUrl page
         *          OR
         *      View(...)                                : call "ExternalLoginConfirmation" method
         */
        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { Email = result.UserName, ExternalLoginData = loginData });
            }
        }


        /*
         *  METHOD      : ExternalLoginConfirmation
         *  DESCRIPTION : 
         *      This method does a POST request method confirm user login
         *  PARAMETERS  :
         *      RegisterExternalLoginModel model : Model holding database of external logins
         *      string returnUrl                 : URL string, indicating return page
         *  RETURNS     :
         *      RedirectToAction("Manage") : returns to "Manage" web page
         *          OR
         *      RedirectToLocal(returnUrl) : returns to previously visited web page
         *          OR
         *      View()                     : returns to previously visited web page along with model
         */
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (BookstoreContext db = new BookstoreContext())
                {
                    User user = db.User.FirstOrDefault(u => u.Email.ToLower() == model.Email.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.User.Add(new User { Email = model.Email });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.Email);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "Email Address already exists. Please enter a different Email Address.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        /*
         *  METHOD      : ExternalLoginFailure
         *  DESCRIPTION : 
         *      This method does a GET request method to show the "failed to login" page
         *  PARAMETERS  : N/A
         *  RETURNS     :
         *      View()  : Opens up the page
         */
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        /*
         *  METHOD      : ExternalLoginsList
         *  DESCRIPTION : 
         *      This method does a POST request method show the user the partial
         *      external login page
         *  PARAMETERS  :
         *      string returnUrl : web page to open next
         *  RETURNS     :
         *      PartialView()  : Get the PartialViewResult for the
         *                       "_ExternalLoginsListPartial" page
         */
        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        /*
         *  METHOD      : RemoveExternalLogins
         *  DESCRIPTION : 
         *      This method does a GET request method removes the external logins
         *  PARAMETERS  : N/A
         *  RETURNS     :
         *      PartialView()  : Get the PartialViewResult for the
         *                       "_RemoveExternalLoginsPartial" page
         */
        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        /*
       *  METHOD      : Register
       *  DESCRIPTION : 
       *      This method does a GET request method to register the user 
       *      into the database if they have the correct Email ending.  
       *  PARAMETERS  : N/A
       *  RETURNS     :
       *      View()  : Sends the user back to the Index/Home page
       */
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ChangePassword(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.ReturnUrl = Url.Action("ChangePassword");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                model.NewPassword = Crypto.HashPassword(model.NewPassword);

                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded = false;
                //check if chnage password worked
                try
                {
                    //check if old password is the same
                     User resultUser = db.User.Where(u => (u.Email == User.Identity.Name)).FirstOrDefault();
                    bool verifyPassword = Crypto.VerifyHashedPassword(resultUser.pword, model.OldPassword);
                    if (verifyPassword)
                    {
                        string resetToken = WebMatrix.WebData.WebSecurity.GeneratePasswordResetToken(User.Identity.Name);
                        changePasswordSucceeded = WebSecurity.ResetPassword(resetToken, model.NewPassword);
                    }
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    var userId = WebSecurity.GetUserId(User.Identity.Name);
                    //create an array of bytes we will use to store the encrypted password
                    User user = db.User.Where(u => u.UserID == userId).FirstOrDefault();

                    ////set hashed password
                    user.pword = model.NewPassword;
                    db.SaveChanges();
                    AuditController.AuditEntry(userId, userId, AuditController.MODIFY_USER, "Password", model.OldPassword, model.NewPassword);
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
             
            }
            else
            {
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #region Helpers
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

        public static string SetSecurityLevel(string user)
        {
            string i = "";
            if (user.Contains("@conestogac.on.ca"))
            {
                if (user.Contains("-cc@conestogac.on.ca"))
                {

                    i ="general";
                }
                else
                {
                    i = "admin";
                }
                
            }
            if (user.Contains("@wlu.ca"))
            {
                i = "general";
            }
            if (user.Contains("@uwaterloo.ca"))
            {
                i = "general";
            }

            return i;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "E-mail already exists. Please enter a different E-mail.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

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
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}