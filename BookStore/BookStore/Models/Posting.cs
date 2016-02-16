using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace BookStore.Models
{
    public class Posting
    {
        [Key]
        [Required]
        public int PostingID { set; get; }

        [Required]
        public int UserID { set; get; }

        [Required]
        public int Institution_ID { set; get; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Book Title:")]
        [RegularExpression(@"^([-A-Za-z' ])+", ErrorMessage = "Book Title must only contain uppercase lowercase aplhabets")]
        public string BookTitle { set; get; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Author:")]
        [RegularExpression(@"^([-A-Za-z' ])+", ErrorMessage = "Author must only contain uppercase lowercase aplhabets")]
        public string author { set; get; }

        [checkSingularDates]
        [DataType(DataType.Date)]
        public Nullable<DateTime> Posting_Date { set; get; }

        [checkSingularDates]
        [DataType(DataType.Date)]
        public Nullable<DateTime> ExpiryDate { set; get; }

        
        [StringLength(255)]
        [Display(Name = "Description: ")]
        public string PostDescription { set; get; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Post Title:")]
        public string PostTitle { set; get; }

        [Required]
        [RegularExpression(@"^[\d.]+$", ErrorMessage = "Price can only contain numbers")]
        [Display(Name = "Price:")]
        public Nullable<Double> price { set; get; }

        public string img { get; set; }
       
    }



    public class OfficialPosting 
    {
        [Key]
        [Required]
        public int PostingID { set; get; }

        [Display(Name = "CourseID: ")]
        public  string CourseID { set; get; }


        [StringLength(255)]
        [Display(Name = "CourseInfo: ")]
        public string CourseInfo { set; get; }

    }

}