using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.Models
{
    public class CreditDetail
    {
        public string memberID { get; set; }
        [Required]
        public int creditAmount { get; set; }
    }
}
