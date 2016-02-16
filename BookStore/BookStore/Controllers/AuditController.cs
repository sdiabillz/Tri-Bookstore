using BookStore.DAL;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    /*  
      *  NAME    : AuditController
      *  PURPOSE : The methods in this class track usage and records it the audit
      *            table each time a specific action takes place.  If a user attribute 
      *            is modified, the audit table will take note.  It keeps track of user
      *            changes, their time cards, and saves the information into a database.
      */
    [Authorize]
    public class AuditController : Controller
    {

        public static string MODIFY = "MODIFY POST";
        public static string CREATE = "ADD POST";
        public static string REMOVE = "REMOVE POST";

        public static string CREATEOFFICIAL = "ADD OFFICIAL POST";
        public static string REMOVEOFFICIAL = "REMOVE OFFICIAL POST";
        public static string MODIFYOFFICIAL = "MODIFY OFFICIAL POST";

        public static string ADD_USER = "ADD USER";
        public static string MODIFY_USER = "MODIFY USER";

        private BookstoreContext db = new BookstoreContext();



        /*
         *  METHOD      : AuditEntry
         *  DESCRIPTION : 
         *      This method begins an entry into the audit table starting from inactivity
         *  PARAMETERS  :
         *      int userId    : logged in user's ID
         *      int PostID     : ID of post being created
         *      string action : create/modify/delete
         *  RETURNS     :
         *      Audit auditEntry.ID : the ID integer of the entry being recorded
         */
        public static int AuditEntry(int userId, int PostID, string action)
        {

            using (var context = new BookstoreContext())
            {
                var auditEntry = new Audits
                {
                    ActionTime = DateTime.Now,
                    User_ID = userId,
                    Posting_ID = PostID,
                    Action = action,

                };
                context.Audit.Add(auditEntry);
                context.SaveChanges();
                SaveDBcontext(context);
                return auditEntry.ID;
            }

        }


        /*
         *  METHOD      : AuditEntry
         *  DESCRIPTION : 
         *      When modifying a Posting, this method compares the two
         *      and records the differences.  Specifically, the user attributes.
         *  PARAMETERS  :
         *      int userId           : user logged in
         *      Employee oldPost : Post being modified
         *      Employee newPost : "new" Post (being modified)
         *      int originInstitutionID  : The Institution ID the old post belonged to
         *  RETURNS     : N/A
         */
        public static void AuditEntry(int userId, Posting oldPost, Posting newPost)
        {


            using (var context = new BookstoreContext())
            {

                if (oldPost != null && newPost != null)
                {
                    // general post's attributes
                    if (oldPost.author != newPost.author)
                    {
                        AuditEntry(userId, newPost.PostingID, MODIFY, "AUTHOR", oldPost.author, newPost.author);
                    }
                    if (oldPost.BookTitle != newPost.BookTitle)
                    {
                        AuditEntry(userId, newPost.PostingID, MODIFY, "BOOK TITLE", oldPost.BookTitle, newPost.BookTitle);
                    }
                    if (oldPost.PostTitle!= newPost.PostTitle)
                    {
                        AuditEntry(userId, newPost.PostingID, MODIFY, "POST TILTE", oldPost.PostTitle, newPost.PostTitle);
                    }
                    if (oldPost.PostDescription != newPost.PostDescription)
                    {
                        AuditEntry(userId, newPost.PostingID, MODIFY, "POST DESCRIPTION", oldPost.PostDescription, newPost.PostDescription);
                    }
                    if (oldPost.price != newPost.price)
                    {
                        AuditEntry(userId, newPost.PostingID, MODIFY, "PRICE", oldPost.price.ToString(), newPost.price.ToString());
                    }
                }

            }

        }

        /*
         *  METHOD      : AuditEntry
         *  DESCRIPTION : 
         *      Check the previous account user and new account user information,
         *      record the differences.  Checks to see if PASSWORD, Securitylevel, or
         *      Email has changed.
         *  PARAMETERS  :
         *      int userId      : user logged in
         *      User oldUser    : previous user
         *      User newUser    : new user
         *      bool isCompare  : (previously used) comparing factor
         *  RETURNS     :
         *      bool ret        : Once finished adding audit entry, return false.
         */
        public static bool AuditEntry(int userId, User oldUser, User newUser, bool isCompare = false)
        {
            bool ret = false;
            if (!oldUser.Equals(newUser))
            {
                if (oldUser.pword != newUser.pword)
                {

                    AuditEntry(userId, oldUser.UserID, MODIFY_USER, "pword", oldUser.pword.ToString(), newUser.pword.ToString());

                }
                if (oldUser.Securitylevel != newUser.Securitylevel)
                {
                    AuditEntry(userId, oldUser.UserID, MODIFY_USER, "Securitylevel", oldUser.Securitylevel.ToString(), newUser.Securitylevel.ToString());

                }
                if (oldUser.Email != newUser.Email)
                {
                    AuditEntry(userId, oldUser.UserID, MODIFY_USER, "Email", oldUser.Email.ToString(), newUser.Email.ToString());

                }
            }
            return ret;

        }


        /*
         *  METHOD      : AuditEntry
         *  DESCRIPTION : 
         *      The end of the audit entry, add it into the audit table.
         *  PARAMETERS  :
         *      int userId        : user logged in
         *      int PostID         : Post ID (Post modified)
         *      string action     : one of static private strings above, denoting which action performed
         *      string attr       : attribute that was changed
         *      string oldValue   : attribute's old value
         *      string newValue   : attribute's new value
         *  RETURNS     :
         *      int auditEntry.ID : The new audit entry's traceable ID
         */
        public static int AuditEntry(int userId, int PostID, string action, string attr, string oldValue, string newValue)
        {

            using (var context = new BookstoreContext())
            {

                var auditEntry = new Audits
                {
                    ActionTime = DateTime.Now,
                    User_ID = userId,
                    Posting_ID = PostID,
                    Action = action,
                    Attribute_Name = attr,
                    Old_value = oldValue,
                    New_Value = newValue
                };

                context.Audit.Add(auditEntry);
                context.SaveChanges();

                return auditEntry.ID;
            }
        }

        /*
         *  METHOD      : Index
         *  DESCRIPTION : 
         *      GET request method to open auditing view page
         *  PARAMETERS  : N/A
         *  RETURNS     :
         *      ActionResult view(audit) : web page given the audit model to display information
         */
        public ActionResult Index()
        {
            return View(db.Audit.ToList());
        }



        /*
         *  METHOD      : Details
         *  DESCRIPTION : 
         *      Checks to see if audit table is empty, if not then display within view.
         *  PARAMETERS  :
         *      int id              : the id of the audit entry
         *  RETURNS     :
         *      ActionResult(audit) : Opens a web page with more specific details
         */
        public ActionResult Details(int id = 0)
        {
            Audits audit = db.Audit.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            return View(audit);
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




        /*
         *  METHOD      : SaveDBcontext
         *  DESCRIPTION : 
         *      Audit was successful, modification/change was valid, now save
         *      this information into the database
         *  PARAMETERS  :
         *      EmployeeDBContext context : stores the employee information
         *  RETURNS     : N/A
         */
        private static void SaveDBcontext(BookstoreContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                //// Throw a new DbEntityValidationException with the improved exception message.
                //throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

    }
}