using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore.DAL;
using BookStore.Models;
using WebMatrix.WebData;
using System.Data.Entity;

namespace BookStore.Controllers
{
    /* 
    *  NAME    : UserController
    *  PURPOSE : The methods in this class handle the action managing user of the website.
    *              The user of this controller is usually the web master who has more 
     *              priviledges than the other adminand general users. 
     *              
     * NOTE: This Function has not been fully implemented yet and this controller is never used. For future Work
    */
     [Authorize]
    public class UserController : Controller
    {
        private static BookstoreContext db = new BookstoreContext();

         

        /*
         *  METHOD      : Index
         *  DESCRIPTION : Displays the list of all current users
         *                accounts within the database related to an institution.  GET request method.
         *  PARAMETERS  : N/A
         *  RETURNS     :
         *      ActionResult View(db.Users.ToList()) :
         */
        public ActionResult Index()
        {
            //find which instituiton first, then send by institution
            
            return View(db.User.ToList());
        }


        /*
         *  METHOD      : Details
         *  DESCRIPTION : Displays the details of a specified account users
         *                within the database.  GET request method.
         *  PARAMETERS  : 
         *      string id  : default null, userID to search within system
         *  RETURNS     :
         *      ActionResult HttpNotFound : error page
         *          OR
         *      ActionResult View(user)   : details page
         */
        public ActionResult Details(string id = null)
        {
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        /*
         *  METHOD      : Create
         *  DESCRIPTION : Creates a specified account users
         *                within the database.  GET request method.
         *  PARAMETERS  : N/A
         *  RETURNS     :
         *      ActionResult View()   : Goes to creation page
         */
        public ActionResult Create()
        {
            return View();
        }

        /*
         *  METHOD      : Create
         *  DESCRIPTION : 
         *      POST request method.  Displays all details (attributes) of a specific 
         *      account user to be inputted.  Displays the errors on invalid fields.
         *      Returns to "create" main page if failed.  Otherwise, the account username
         *      is added to user table model.
         *  PARAMETERS  :
         *      User user               : New accounter user created
         *  RETURNS     :
         *      ActionResult RedirectToAction("Index") : Returns to the "index" (main) creation page 
         *          OR
         *      ActionResult View(user) : User has been validated
        // */
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.User.Add(user);
        //        db.SaveChanges();

        //        var userId = WebSecurity.GetUserId(User.Identity.Name);                     // AUDIT
        //        var userIdCreated = user.UserID;
        //        //AuditController.AuditEntry(userId, userIdCreated, AuditController.ADD_USER);    

        //        return RedirectToAction("Index");
        //    }

        //    return View(user);
        //}


        /*
         *  METHOD      : Edit
         *  DESCRIPTION : 
         *      GET request method.  After selecting a account user from the list,
         *      open the edit form to make adjustments.  
         *  PARAMETERS  :
         *      int id : The id of the employee to be edited
         *  RETURNS     :
         *      ActionResult HttpNotFound() : Error page
         *          OR
         *      ActionResult View(user) : User has been found and is to be edited.
         */
        public ActionResult Edit(int id)
        {
            //NOTE ADD NAME AND LASTNAME AND PHONE NUMBER to USERS TABLE and 
            //USERS CAN EDIT THESW DETAILS
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }



        /*
         *  METHOD      : Edit
         *  DESCRIPTION : 
         *      POST request method.  Displays all details (attributes) of a specific 
         *      account user.  Displays any incorrect inputs and errors.  Provide a 
         *      "return to index" link.  Change state to inactive and make edits. 
         *  PARAMETERS  :
         *      User user   : User trying to get added.
         *  RETURNS     :
         *      ActionResult RedirectToAction("Index") : Return to edit main menu
         *          OR
         *      ActionResult View(user)    : The newly updated model
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                // log modify action
                var userId = WebSecurity.GetUserId(User.Identity.Name);
                // AuditController.AuditEntry(userId, user.UserID, AuditController.MODIFY_USER);                   // AUDIT

                return RedirectToAction("Index");
            }
            return View(user);
        }

        /*
         *  METHOD      : Dispose
         *  DESCRIPTION : 
         *      Cleans up database
         *  PARAMETERS  :
         *      bool disposing : to cleanse or not to cleanse
         *  RETURNS     : N/A
         */
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

         public static int GetInstituion( string user)
         {
             int i = 0;
            if (user.Contains("@conestogac.on.ca"))
            {
                i = 2;
            }
            if (user.Contains("@wlu.ca"))
            {
                i = 1;
            }
            if (user.Contains("@uwaterloo.ca"))
            {
                i = 3;
            }

             return i;
         }
          
         /*
         *  METHOD      : GetDropDown()
         *  DESCRIPTION : 
         *      Populates a drop-down list from the database of companies 
         *      (Used for web forms to select a company whenever prompt for companies)
         *  PARAMETERS  : N/A
         *  RETURNS     :
         *      List<SelectListItem> ls : List of all companies within database
         */
        public static IEnumerable<SelectListItem> GetDropDown()
        {
            List<SelectListItem> ls = new List<SelectListItem>();
            var companies = db.Institutions;
            foreach (var company in companies)
            {
                ls.Add(new SelectListItem() { Text = company.Name, Value = company.Institution_ID.ToString() });
            }
            return ls;
        }
           
    }
}