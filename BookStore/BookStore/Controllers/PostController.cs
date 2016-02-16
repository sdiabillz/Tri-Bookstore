using BookStore.DAL;
using BookStore.Models;
using BookStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace BookStore.Controllers
{
    /*  
  *  NAME    : PostController
  *  PURPOSE : The methods in this class handle the action Posting a book.
*              if the user wishes to post their postings, edit them, the controller takes care
*              of their action. It makes use of a view model and passes data to 
*              its views using its view Model.
  */
    [InitializeSimpleMembership]
    [Authorize]
    public class PostController : Controller
    {
        private static BookstoreContext db = new BookstoreContext();


        // GET: Post
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index(string ReturnUrl)
        {
            var postingViewModel = new PostingsViewModel { PostingType = "NORMAL" };     
            return View(postingViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index( PostingsViewModel postingModel)
        {
            if (ModelState.IsValid)
            {
                // Attempt to add a new post
                try
                {
                    if (postingModel.ListName == null)
                    {                        
                       //Get infor for posting adn set it
                        var userId = WebSecurity.GetUserId(User.Identity.Name);
                        postingModel.postings.UserID = userId;
                        postingModel.postings.Posting_Date = DateTime.Now;
                        postingModel.postings.ExpiryDate = DateTime.Now.AddMonths(3);
                                           
                        //get institution from email.
                        postingModel.postings.Institution_ID = UserController.GetInstituion(User.Identity.Name);

                        if (postingModel.filepath != null)
                        {
                            //var uploadDir = "~/uploads";
                            //var imagePath = Path.Combine(Server.MapPath(uploadDir), postingModel.filepath.FileName);
                            //imageUrl = Path.Combine(uploadDir, postingModel.filepath.FileName);
                            //postingModel.filepath.SaveAs(imagePath);
                            //ViewBag.Logo = imageUrl;
                            //postingModel.postings.img = imageUrl;

                        }
                       // int id = 
                        db.Postings.Add(postingModel.postings);//.PostingID;

                        //if (User.IsInRole("admin"))
                        //{
                        //    if (postingModel.isOfficial)
                        //    {
                        //        postingModel.Opostings.PostingID = id;
                        //        db.OfficialPostings.Add(postingModel.Opostings);
                        //    }
                        //}

                        db.SaveChanges();
                    }
                                       
                }
                catch (DbEntityValidationException  e)
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
                var postId = postingModel.postings.PostingID;
                 AuditController.AuditEntry(userIds, postId, AuditController.CREATE);
              
                return RedirectToAction("PostPreview", "Post", postingModel.postings);
            }
            return View(postingModel.postings.ToPostingViewModel());
        }

   

        //public ActionResult Upload(HttpPostedFileBase file, PostingsViewModel postingModel)
        //{
        //    if (file != null)
        //    {
        //        /*Geting the file name*/
        //        string filename = System.IO.Path.GetFileName(file.FileName);
        //        /*Saving the file in server folder*/
        //        file.SaveAs(Server.MapPath("~/Images/" + filename));
        //        string filepathtosave = "Images/" + filename;
        //        /*Storing image path to show preview*/
        //        ViewBag.ImageURL = filepathtosave;
        //        postingModel.filepath = file;

        //    }
        //    // Back to form 
        //    return View(postingModel.postings.ToPostingViewModel());
        //}


        // GET: Post

        [ActionName("PostPreview")]
        public ActionResult PostPreview(Posting post)
        {
            if (post == null)
                post = new Posting();

            return View(post);
            // return View();
        }

        // GET: Post
        [ActionName("BackToLogin")]
        public ActionResult BackToLogin()
        {
            return RedirectToAction("Login", "Account");
           // return View();
        }

        public ActionResult Details(int id = 0)
        {
            Posting post = db.Postings.Find(id);
            if (post == null)
            {
                post = new Posting();
            }           
            return View(post);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Edit(int id = 0)
        {
            Posting post = db.Postings.Find(id);
            if (post == null)
            {
                post = new Posting();
            }
            var postingViewModel = new PostingsViewModel { PostingType = "NORMAL" };
            postingViewModel.postings = post;
            return View(postingViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PostingsViewModel postingModel, int id = 0)
        {
            if (ModelState.IsValid)
            {
              
                try
                {
                    // find original Posting
                    postingModel.postings.PostingID = id;
                    var OriginalPost = db.Postings.Find(id);
                    
                    var oldPost = new Posting                                           // AUDIT 
                    {
                        UserID = postingModel.postings.UserID,
                        Posting_Date = postingModel.postings.Posting_Date,
                        ExpiryDate = postingModel.postings.ExpiryDate,
                        PostTitle = postingModel.postings.PostTitle,
                        PostDescription = postingModel.postings.PostDescription,
                        price = postingModel.postings.price,
                        img = postingModel.postings.img,
                        BookTitle = postingModel.postings.BookTitle,
                        author = postingModel.postings.author,
                        Institution_ID = postingModel.postings.Institution_ID,
                       
                    };

                   // UpdateExistingPosting(OriginalPost, postingModel.postings);
                    OriginalPost.PostTitle = postingModel.postings.PostTitle;
                    OriginalPost.PostDescription = postingModel.postings.PostDescription;
                    OriginalPost.price = postingModel.postings.price;
                    OriginalPost.img = postingModel.postings.img;
                    OriginalPost.BookTitle = postingModel.postings.BookTitle;
                    OriginalPost.author = postingModel.postings.author;

                    db.Entry(OriginalPost).State = EntityState.Modified;
                    db.SaveChanges();

                     //Add Audit
                    var userIds = WebSecurity.GetUserId(User.Identity.Name);
                    AuditController.AuditEntry(userIds, oldPost, OriginalPost);
                 
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

                return RedirectToAction("PostPreview2", "Post", postingModel.postings);
            }

            return View(postingModel.postings.ToPostingViewModel());
        }

        [ActionName("PostPreview2")]
        public ActionResult PostPreview2(Posting post)
        {
            if (post == null)
                post = new Posting();

            return View(post);
            // return View();
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
        #endregion
    }

}