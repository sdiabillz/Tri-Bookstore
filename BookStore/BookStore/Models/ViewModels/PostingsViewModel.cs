/*  
 *  FILE          : PostingsViewModel.cs
 *  PROJECT       : Tri-Bookstore
 *  DESCRIPTION   : This is a view model that a post can be transformed into for posting books.
 *                  it contains all the required data that needs to be passed from the 
 *                  PostController to the Post Views. The view model can also be turned back into a post.
 *              
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.WebPages.Html;

namespace BookStore.Models.ViewModels
{
    public class PostingsViewModel
    {

        public string PostingType { get; set; }                // Institution ID, will be modified by the Instotution select list

        public string ListName { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase filepath { get; set; }

        public string institution { get; set; }

        public string UserName { get; set; }

       public virtual ICollection<AssignedInstitution> institutions { get; set; }

       public OfficialPosting Opostings { get; set; }
        public Posting postings { get; set; }
        public Boolean isOfficial { get; set; }

    }

    /*  
 *  NAME    : AssignedInstitution
 *  PURPOSE : This class is used to control which Institutions should be included 
 *            within a list, and without duplicates.  For example, the drop-down 
 *            list of all companies.  No methods are used.
 */
    public class AssignedInstitution
    {
        public int Institution_ID { set; get; }
        public string Institution_Name { set; get; }

        public bool Assigned { get; set; }
    }
}

