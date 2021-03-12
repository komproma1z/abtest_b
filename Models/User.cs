using System;
using System.ComponentModel.DataAnnotations;

namespace Abtest.Models
{
    public class User  {
        public int Id {get; set;}
        [Required]
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? LastActivityDate { get; set; }
        
        public User() {}
    }
}