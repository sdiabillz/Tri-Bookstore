using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.DAL;
using WebMatrix.WebData;
using System.Data.Entity.Validation;
using System.Data.Entity;
namespace BookStore.Controllers
{
    /*  
     *  NAME    : ManageController
     *  PURPOSE : The methods in this class handle Posting information for the user.
   *              if the user wishes to view their postings, edit them, the controller takes care
   *              of their action. It makes use of a view model and passes data to 
   *              its views using its view Model.
     */
    [InitializeSimpleMembership]
    [Authorize]
    public class ManageController : Controller
    {
        public static BookstoreContext db = new BookstoreContext();

        public ActionResult Index(string booktitle, string author, ManageViewModel mine)
        {
            BookstoreContext dba = new BookstoreContext();
            //Get infor for posting adn set it
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            //get institution from email.
            mine.InstitutionID = UserController.GetInstituion(User.Identity.Name);

            if (mine == null)
            {
                mine = new ManageViewModel();

            } 

            List<Posting> postings = null;
           

            //get list of all posting from an instituiton for the admin
            if (User.IsInRole("admin"))
            {
              
                if ((!String.IsNullOrEmpty(booktitle)) && (!String.IsNullOrEmpty(author)))
                {
                    postings = (from ec in dba.Postings
                                where ec.BookTitle.Contains(booktitle) && ec.author.Contains(author) && ec.Institution_ID == mine.InstitutionID && DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0
                                select ec).ToList<Posting>();
                }
                else if ((String.IsNullOrEmpty(booktitle)) && (!String.IsNullOrEmpty(author)) )
                {
                    postings = (from ec in dba.Postings

                                where ec.author.Contains(author) && ec.Institution_ID == mine.InstitutionID && DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0
                                select ec).ToList<Posting>();
                }
                else if ((!String.IsNullOrEmpty(booktitle)) && (String.IsNullOrEmpty(author)))
                {
                    postings = (from ec in dba.Postings
                                where ec.BookTitle.Contains(booktitle) && ec.Institution_ID == mine.InstitutionID && DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0
                                select ec).ToList<Posting>();
                }
                else if ((String.IsNullOrEmpty(booktitle)) && (String.IsNullOrEmpty(author)))
                {
                    postings = (from ec in dba.Postings

                                where ec.Institution_ID == mine.InstitutionID && DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0
                                select ec).ToList<Posting>();
                }
            }

            if (postings == null)
            {
                postings = new List<Posting>();
            }

            //set the value foreach of the postings 
            foreach (Posting ec in postings)
            {
                ec.UserID = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().UserID;
                ec.PostTitle = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().PostTitle;
                ec.PostDescription = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().PostDescription;

                ec.price = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().price;

                ec.author = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().author;
                ec.BookTitle = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().BookTitle;

                ec.PostingID = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().PostingID;
                ec.Posting_Date = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().Posting_Date;

                ec.Institution_ID = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().Institution_ID;
            }

            var postingViewModels = postings.Select(post => post.ToOfficialPostingViewModel()).ToList();
            mine.AllPostings = postingViewModels;

           // List<OfficialPosting> Opostings = null;
            
            return View(mine);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult OfficialBook(int id = 0)
        {
            BookstoreContext dba = new BookstoreContext();
            Posting post = dba.Postings.Find(id);
            if (post == null)
            {
                post = new Posting();
            }

            var manageViewModel = new ManageViewModel();
            manageViewModel.postings = post;
            return View(manageViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("OfficialBook")]
        public ActionResult OfficialBook(ManageViewModel mine, int id = 0)
        {
            BookstoreContext dba = new BookstoreContext();
            if (ModelState.IsValid)
            {
                try
                {
                    if (mine.isOfficial)
                    {
                        mine.Officialpostings.PostingID = id;
                        dba.OfficialPostings.Add(mine.Officialpostings);
                    }
                   
                    dba.SaveChanges();

                }
                catch (DbEntityValidationException e)
                {
                    //display all the validation error on the postingview model
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                }

                //Add Audit
                var userIds = WebSecurity.GetUserId(User.Identity.Name);
                var postId = id;
                AuditController.AuditEntry(userIds, postId, AuditController.CREATEOFFICIAL);
                return RedirectToAction("Index", "manage", mine );
            }
            return View(mine.postings.ToPostingViewModel());
        }

       [HttpGet]
        [ActionName("UNOfficialBook")]
        [AllowAnonymous]
        public ActionResult UNOfficialBook(ManageViewModel mine, int id = 0)
        {
            BookstoreContext dba = new BookstoreContext();
            OfficialPosting tmp = (from ec in dba.OfficialPostings
                                    where ec.PostingID == id
                                    select ec).FirstOrDefault();

            try
            {
                if (tmp != null)
                {
                    dba.OfficialPostings.Remove(tmp);
                }
                dba.SaveChanges();
            }
            catch (Exception e)
            {
               
            }
            //Add Audit
            var userIds = WebSecurity.GetUserId(User.Identity.Name);
            var postId = id;
            AuditController.AuditEntry(userIds, postId, AuditController.REMOVEOFFICIAL);
            return RedirectToAction("Index", "manage", mine);
        }

        //get all post that the user made
       [AllowAnonymous]
       [HttpGet]
       [ActionName("Manage")]
       public ActionResult Manage(ManageViewModel mine)
       {
           BookstoreContext dba = new BookstoreContext();
           //Get infor for posting adn set it
           var userId = WebSecurity.GetUserId(User.Identity.Name);
           mine.InstitutionID = GetInstituion(User.Identity.Name);

           if (mine == null)
           {
               mine = new ManageViewModel();

           }

           List<Posting> postings = null;            
            postings = (from ec in dba.Postings
                        where ec.Institution_ID == mine.InstitutionID && ec.UserID == userId
            select ec).ToList<Posting>();

           if (postings == null)
           {
               postings = new List<Posting>();
           }

           //set the value foreach of the postings 
           foreach (Posting ec in postings)
           {
               ec.UserID = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().UserID;
               ec.PostTitle = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().PostTitle;
               ec.PostDescription = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().PostDescription;

               ec.price = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().price;

               ec.author = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().author;
               ec.BookTitle = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().BookTitle;

               ec.PostingID = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().PostingID;
               ec.Posting_Date = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().Posting_Date;

               ec.Institution_ID = dba.Postings.Where(e => e.PostingID == ec.PostingID).FirstOrDefault().Institution_ID;
           }

           var postingViewModels = postings.Select(post => post.ToOfficialPostingViewModel()).ToList();
           mine.AllPostings = postingViewModels;
           return View(mine);
       }

       public static int GetInstituion(string user)
       {
           int i = 0;
           if (user.Contains("conestogac.on.ca"))
           {
               i = 2;
           }
           if (user.Contains("laurier"))
           {
               i = 1;
           }
           if (user.Contains("waterloo"))
           {
               i = 3;
           }

           return i;
       }

       public ActionResult Delete(int id = 0)
       {
           BookstoreContext db = new BookstoreContext();
           Posting post = db.Postings.Find(id);
           if (post == null)
           {
               post = new Posting();
           }
           try
           {
               OfficialPosting opost = db.OfficialPostings.Find(id);
               if (opost != null)
               {
                   db.OfficialPostings.Remove(opost);
               }

               db.Postings.Remove(post);
               db.SaveChanges();
           }
           catch (Exception e)
           {

           }
           //Add Audit
           var userIds = WebSecurity.GetUserId(User.Identity.Name);
           var postId = id;
           AuditController.AuditEntry(userIds, postId, AuditController.REMOVEOFFICIAL);
           return RedirectToAction("Manage", "manage");
       }

       [ActionName("EditOfficialBook")]
       [HttpGet]
       public ActionResult EditOfficialBook(int id = 0)
       {
           Posting post = db.Postings.Find(id);
           if (post == null)
           {
               post = new Posting();
           }
           OfficialPosting Opost = db.OfficialPostings.Find(id);
           if (Opost == null)
           {
               Opost = new OfficialPosting();
           }
           var postingViewModel = new PostingsViewModel { PostingType = "OFFICIAL" };
           postingViewModel.Opostings = Opost;
           postingViewModel.postings = post;
           return View(postingViewModel);
       }

       [HttpPost]
       [ActionName("EditOfficialBook")]
       [ValidateAntiForgeryToken]
       public ActionResult EditOfficialBook(PostingsViewModel postingModel, int id = 0)
       {
           if (ModelState.IsValid)
           {
                string oldId = "", oldInfo ="";
               try
               {
                   // find original Posting
                   postingModel.Opostings.PostingID = id;
                   var OriginalPost = db.OfficialPostings.Find(id);

                   // UpdateExistingPosting(OriginalPost, postingModel.postings);
                  oldId =  OriginalPost.CourseID;
                   oldInfo = OriginalPost.CourseInfo;
                   OriginalPost.CourseID = postingModel.Opostings.CourseID;
                   OriginalPost.CourseInfo = postingModel.Opostings.CourseInfo;
                   
                   db.Entry(OriginalPost).State = EntityState.Modified;

                   db.SaveChanges();
               }
               catch (DbEntityValidationException e)
               {
                   //display all the validation error on the postingview model
                   foreach (var eve in e.EntityValidationErrors)
                   {
                       Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                           eve.Entry.Entity.GetType().Name, eve.Entry.State);
                       foreach (var ve in eve.ValidationErrors)
                       {
                           Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                               ve.PropertyName, ve.ErrorMessage);
                       }
                   }
               }

               //Add Audit
               var userIds = WebSecurity.GetUserId(User.Identity.Name);
               var postId = id;

               AuditController.AuditEntry(userIds, postId, AuditController.MODIFYOFFICIAL,"COURSEID" ,oldId, postingModel.Opostings.CourseID);
               AuditController.AuditEntry(userIds, postId, AuditController.MODIFYOFFICIAL, "COURSEINFO", oldInfo, postingModel.Opostings.CourseInfo);

               return RedirectToAction("Index", "manage");
           }
           return View(postingModel.postings.ToOfficialPostingViewModel());
       }
    }
}