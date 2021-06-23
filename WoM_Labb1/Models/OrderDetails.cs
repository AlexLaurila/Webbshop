using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WoM_Labb1.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }


        public virtual int orderId { get; set; }
        public int Id { get; set; }
        public virtual Produkt Produkt { get; set; }
        public int Antal { get; set; }
        public int Pris { get; set; }
    }
}
