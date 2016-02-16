using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    /*  
  *  NAME    : Audit
  *  PURPOSE : Logging class holding onto old & new values to reverse changes,
  *            names, IDs, nothing untraceable.  
  */
    public class Audits
    {
        [Key]
        public int ID { set; get; }
        public DateTime ActionTime { set; get; }
        public int User_ID { set; get; }
        public int Posting_ID { set; get; }
        //create delete edit
        [StringLength(20)]
        public string Action { set; get; }
        [StringLength(50)]
        public string Attribute_Name { set; get; }
        [StringLength(200)]
        public string Old_value { set; get; }
        [StringLength(200)]
        public string New_Value { set; get; }
    }
}