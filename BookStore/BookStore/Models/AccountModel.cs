using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.Models
{ 
    /*  
     *  NAME    : RegisterExternalLoginModel
     *  PURPOSE : This class holds the Email and ExternalLoginData strings from the
     *            database.  This information is required to login to the website.
     */
    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string ExternalLoginData { get; set; }
    }


    /*  
     *  NAME    : LocalPasswordModel
     *  PURPOSE : This class holds onto the two passwords required when attempting to
     *            register an user-account for the Bookstore.  As well when changing passwords,
     *            the passwords need to be locally/temporarily stored and checked for errors.
     *            This class holds three strings: old, new, and confirmation passwords.
     */
    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }


    /*  
     *  NAME    : LoginModel
     *  PURPOSE : This model is made to hold the user names & passwords pulled out from
     *            the database.  As a bonus, it checks if a user wishes to autologin (remember password).
     */
    public class LoginModel
    {
        [Required]
        [Display(Name = "Email:")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    /*  
     *  NAME    : RegisterModel
     *  PURPOSE : This model is made to hold the user name & passwords, as well as the 
     *            security (admin/general) levels when a new user is created.
     */
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Security Level")]
        public string SecurityLevel { get; set; }
    }


    /*  
     *  NAME    : ExternalLogin
     *  PURPOSE : The external login class holds the incoming user's username and user ID
     *            to be checked against the databases' information of valid user accounts.
     *            
     */
    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}