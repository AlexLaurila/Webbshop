using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WoM_Labb1.Models
{
    public class Customer
    {
        [Key]
        public string personnummer { get; set; }
        [Required]
        public string fornamn { get; set; }
        [Required]
        public string efternamn { get; set; }
        [Required]
        public string postadress { get; set; }
        [Required]
        public int postnr { get; set; }
        [Required]
        public string ort { get; set; }
        [Required]
        public string epost { get; set; }
        [Required]
        public string telefonnummer { get; set; }
    }
}
