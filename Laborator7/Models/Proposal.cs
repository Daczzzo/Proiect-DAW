﻿using Laborator4.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Laborator7.Models
{
    public class Proposal
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(20, ErrorMessage = "Titlul nu poate avea mai mult de 20 caractere")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Continutul articolului este obligatoriu")]
        public string Content { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Campul trebuie sa contina data si ora")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }

        public string UserId { get; set; }

        public virtual Category Category { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }


        public virtual ApplicationUser User { get; set; }
    }
}