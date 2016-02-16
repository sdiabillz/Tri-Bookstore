using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.Models
{

    public class Institution
    {
        [Key]
        [Display(Name = "Company ID")]
        public int Institution_ID { set; get; }
          [Display(Name = "Company Name")]
        public string Name { set; get; }

          public virtual ICollection<UserInstitution> UserInstitutions { get; set; }
       // public virtual ICollection<PostandInstitution> PostandInstitutions { get; set; }
    }
}