using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Laborator7.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }

        public int NewsId { get; set; }

        public virtual News News { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}