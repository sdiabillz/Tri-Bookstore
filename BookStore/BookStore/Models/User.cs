using BookStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Models
{

    public class User
    {
        [Key]
        public int UserID { set; get; }

        [Display(Name = "Password")]
        public string pword { set; get; }

        //General or Admin
        public string Securitylevel { set; get; }

        [StringLength(50)]
        [Display(Name = "Email")]
        public string Email { set; get; }


        public virtual ICollection<UserInstitution> UserInstitutions { get; set; }

        //Add First name , last and phone number

    }

    public class UserViewModel
    {
        public string ListName { get; set; }                // company ID, will be modified by the company select list

        public int originCompanyID { get; set; }         // used to find and drop the old employee_company rows when trying to update it
        public string InstitutionName { get; set; }            // used in index view
        public IEnumerable<SelectListItem> ItemSeletList { get; set; }  // company select list

        public string OrigenType { get; set; }
        public string UserType { get; set; }
        public IEnumerable<SelectListItem> UserTypeSelect { get; set; }

        public RegisterModel model { get; set; }

        public virtual ICollection<AssignedInstitution> institutons { get; set; }

        public User user { get; set; }

    }
}