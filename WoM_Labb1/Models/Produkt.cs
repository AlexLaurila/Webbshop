using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WoM_Labb1.Models
{
    public class Produkt
    {
        [Key]
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 5)]
        [Required]
        public string productName { get; set; }

        [StringLength(350, MinimumLength = 10)]
        [Required]
        public string productDescription { get; set; }

        [Range(1, 500)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(10, 2)")]
        [Required]
        public int productPrice { get; set; }

        [Range(1, 200)]
        [Required]
        public int productInStore { get; set; }

        public byte[] productImage { get; set; }
    }
}
