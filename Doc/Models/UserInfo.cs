using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.Models
{
    public class UserInfo
    {
        [Required]
        public string memberID { get; set; }
        [Required]
        public string userName { get; set; }
        public string address { get; set; }
        [Required]
        public DateTime dob { get; set; }
        public string employerName { get; set; }
        public int availableCredits { get; set; }
    }
}
