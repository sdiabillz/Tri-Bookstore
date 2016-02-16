using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class UserInstitution
    {
        [Key]
        public int UserID { set; get; }
        [Key]
        public int Institution_ID { set; get; }

        public string UserType { set; get; }

        [ForeignKey("UserID")]
        public User user { set; get; }

        [ForeignKey("Institution_ID")]

        public Institution institution { set; get; }

    }
}