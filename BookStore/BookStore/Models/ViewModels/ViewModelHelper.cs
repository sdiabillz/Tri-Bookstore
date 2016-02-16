/*  
 *  FILE          : ViewModeHelpers.cs
 *  PROJECT       : Tri-Bookstore
 *  DESCRIPTION   : The methods in this file help with the view modeling as 
 *                  it turns single post into a working view model.
 *                  As well, it can change a post view model back into 
 *                  a post object if needed.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookStore.Controllers;
using WebMatrix.WebData;
using BookStore.DAL;

namespace BookStore.Models.ViewModels
{

    /*  
     *  NAME    : ViewModeHelpers
     *  PURPOSE : This class contains two methods to transform an employee into
     *            a view model.  Each method was required at some point by another
     *            module to progress further.  
     */
    public static class ViewModeHelpers
    {
        public static BookstoreContext db = new BookstoreContext();

        /*
         *  METHOD      : ToViewModel
         *  DESCRIPTION : 
         *      Changes a posting type into a view model.  Regardless of the posting
         *      type, it can be made into a model.  Once an posting is converted, 
         *      the view model is returned.
         *  PARAMETERS  : 
         *      this Posting posting : The posting to transform.
         *  RETURNS     :
         *      postingViewModel : Depends on class; is an PostingViewModel.
         */
        public static PostingsViewModel ToOfficialPostingViewModel(this Posting post)
        {
            BookstoreContext db = new BookstoreContext();
            var PostingsViewModel = new PostingsViewModel();

            PostingsViewModel.postings = (Posting)post;

            OfficialPosting offPost = (from ec in db.OfficialPostings
                                   where ec.PostingID == post.PostingID
                                   select ec).FirstOrDefault();

            if (offPost != null)
            {
                 PostingsViewModel.PostingType = "OFFICIAL";
            }
            else
            {
                 PostingsViewModel.PostingType = "NORMAL";
            }
            // bookViewModel.ItemSeletList = BookController.GetDropDown();         //// add back the reasonForLeave drop-down
            PostingsViewModel.Opostings = (OfficialPosting)offPost;
            string tmpInstitutionName = "";

            foreach (Institution x in db.Institutions)
            {
                if (post.Institution_ID == x.Institution_ID)
                {
                    tmpInstitutionName = x.Name;
                    break;
                }
            }

            PostingsViewModel.institution = tmpInstitutionName;


            return PostingsViewModel;
        }

        public static PostingsViewModel ToPostingViewModel(this Posting post)
        {

            // conver db employee to employeeCompanyViewModel, uded in delete
            var PostingsViewModel = new PostingsViewModel();

            PostingsViewModel.postings = (Posting)post;
            // bookViewModel.ItemSeletList = BookController.GetDropDown();         //// add back the reasonForLeave drop-down
            PostingsViewModel.PostingType = "NORMAL";

            return PostingsViewModel;
        }

        public static UserViewModel ToUserViewModel(this User user)
        {
            // conver db employee to employeeCompanyViewModel, uded in delete
            var usersViewModel = new UserViewModel();

            usersViewModel.user = (User)user;
            usersViewModel.ItemSeletList = UserController.GetDropDown();         //// add back the reasonForLeave drop-down
            usersViewModel.UserType = "general";

            foreach (var company in user.UserInstitutions)
            {
                usersViewModel.institutons.Add(new AssignedInstitution
                {
                    Institution_ID = company.Institution_ID,
                    Institution_Name = company.institution.Name,
                    Assigned = true
                });
            }
            return usersViewModel;
        }

    }
}