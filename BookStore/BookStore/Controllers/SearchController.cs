using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore.Models;
using BookStore.DAL;
using BookStore.Models.ViewModels;
using Microsoft.SqlServer.Server;
namespace BookStore.Controllers
{
    /*  
    *  NAME    : SearchController
    *  PURPOSE : The methods in this class handle the action of searching a book.
    *              if the user wishes to search their postins, the controller takes care
    *              of their action. It makes use of a view model that contains all the searched postings and passes data to 
    *              its views using its view Model.
    */
    [InitializeSimpleMembership]
    [Authorize]
    public class SearchController : Controller
    {
        public static BookstoreContext db = new BookstoreContext();

        /*  
        *  Method   : Index
        *  PURPOSE : The purpose is to direct the user to the search page where user can search items 
        *            by Bootitle and author. It fills the viewmodel with the required or searchd 
         *            book postings and send them to the user for display.
        */
        public ActionResult Index( string booktitle, string author, SearchViewModel mine)
        {
            BookstoreContext dba = new BookstoreContext();
            if (mine == null)
            {
                mine =  new SearchViewModel();

            }
            
            List<Posting> postings = null;

            // if no institution was chosen, get the postings for all institutions
            if (string.IsNullOrEmpty(mine.chosenValue))
            {
                if ((!String.IsNullOrEmpty(booktitle)) && (!String.IsNullOrEmpty(author)))
                {
                    postings = (from ec in dba.Postings
                                where ec.BookTitle.Contains(booktitle) && ec.author.Contains(author) && DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0 
                                select ec).ToList<Posting>();
                }
                else if ((String.IsNullOrEmpty(booktitle)) && (!String.IsNullOrEmpty(author)) ) 
                {
                    postings = (from ec in dba.Postings
                                where ec.author.Contains(author) && DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0
                                select ec).ToList<Posting>();
                }
                else if ((!String.IsNullOrEmpty(booktitle)) && (String.IsNullOrEmpty(author)))
                {
                    postings = (from ec in dba.Postings
                                where ec.BookTitle.Contains(booktitle) && DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0
                                select ec).ToList<Posting>();
                }
                else if ((String.IsNullOrEmpty(booktitle)) && (String.IsNullOrEmpty(author)))
                {
                    postings = (from ec in dba.Postings where  DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0
                                select ec).ToList<Posting>();
                }
            }
            else
            {
                //get the chosen instititution in the search page
                int tmpinstitutionID = 0;

                foreach(Institution x in dba.Institutions)
                {
                    if (mine.chosenValue == x.Name)
                    {
                        tmpinstitutionID = x.Institution_ID;
                        break;
                    }
                }

                if ((!String.IsNullOrEmpty(booktitle)) && (!String.IsNullOrEmpty(author)))
                {
                    postings = (from ec in dba.Postings
                                where ec.BookTitle.Contains(booktitle) && ec.author.Contains(author) && ec.Institution_ID == tmpinstitutionID && DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0
                                select ec).ToList<Posting>();
                }
                else if ((String.IsNullOrEmpty(booktitle)) && (!String.IsNullOrEmpty(author)))
                {
                    postings = (from ec in dba.Postings
                                where ec.author.Contains(author) && ec.Institution_ID == tmpinstitutionID && DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0
                                select ec).ToList<Posting>();
                }
                else if ((!String.IsNullOrEmpty(booktitle)) && (String.IsNullOrEmpty(author)))
                {
                    postings = (from ec in dba.Postings
                                where ec.BookTitle.Contains(booktitle) && ec.Institution_ID == tmpinstitutionID && DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0
                                select ec).ToList<Posting>();
                }
                else if ((String.IsNullOrEmpty(booktitle)) && (String.IsNullOrEmpty(author)))
                {
                    postings = (from ec in dba.Postings
                                where ec.Institution_ID == tmpinstitutionID && DateTime.Compare((DateTime)ec.ExpiryDate, DateTime.Today) > 0
                                select ec).ToList<Posting>();
                }
               
            }
     
            if (postings == null)
            {
                postings = new List<Posting>();
            }

            //set the value foreach of the postings that macth the search 
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

        /*  
        *  Method   : Details
        *  PURPOSE : The purpose is display the details of the serached book. 
         *             the book that is being searched is searched y ID.
        */
        [ActionName("Details")]
        public ActionResult Details(int id = 0)
        {
            Posting post = db.Postings.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        /*  
       *  Method   : Details
       *  PURPOSE : The purpose is display the Post information for the user to contact. 
        *             the userEmail that is being searched is searched by ID.
       */
        [ActionName("ContactPost")]
        public ActionResult ContactPost(int id = 0)
        {
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
    }

     

}