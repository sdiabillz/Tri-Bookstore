/*  
 *  FILE          : SearchViewModel.cs
 *  PROJECT       : Tri-Bookstore
 *  DESCRIPTION   : This is a view model that a post can be transformed into for searching books.
 *                  it contains all the required data that needs to be passed from the 
 *                  searchController to the Search Views. The view model can also be turned back into a post.
 *              
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.Models.ViewModels
{
    public class SearchViewModel
    {       
        public string chosenValue { get; set; }

        public IEnumerable<PostingsViewModel> AllPostings { get; set; }

        public Posting postings { get; set; }

        public OfficialPosting Opostings { get; set; }

        public string UserName { get; set; }

    }

}