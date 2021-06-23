using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WoM_Labb1.Models
{
    public class ShoppingCart
    {
        [Key]
        public int VarukorgId { get; set; }

        public int Id { get; set; }
        public virtual Produkt Produkt { get; set; }

        public int Antal { get; set; }
    }
}
