/*  
 *  FILE          : ManageViewModel.cs
 *  PROJECT       : Tri-Bookstore
 *  DESCRIPTION   : The is a view model that a post can be transformed intofor the user to manage their own postings.
 *                  it contains all the required data that needs to be passed from the 
 *                  ManageController to the Manage View. The view model can also be turned back into a post.
 *              
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models.ViewModels
{

    public class ManageViewModel
    {
        public string chosenValue { get; set; }

        public IEnumerable<PostingsViewModel> AllPostings { get; set; }

        public Posting postings { get; set; }

        public OfficialPosting Officialpostings { get; set; }

        public string UserName { get; set; }

        public int InstitutionID { get; set; }

        public bool isOfficial { get; set; }
    }
}