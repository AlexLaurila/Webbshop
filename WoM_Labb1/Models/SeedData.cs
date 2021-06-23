using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WoM_Labb1.Data;
using System;
using System.Linq;
using System.IO;

namespace WoM_Labb1.Models
{
    /// <summary>
    /// Contains data for products if database happens to be empty upon startup.
    /// </summary>
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ProduktContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ProduktContext>>()))
            {
                if (context.Produkt.Any())
                {
                    return;  
                }

                byte[] bytes;

                FileInfo file = new FileInfo(@".\wwwroot\Images\fotboll.jpg");
                using (BinaryReader reader = new BinaryReader(file.OpenRead()))
                {
                    bytes = reader.ReadBytes(Convert.ToInt32(file.Length));
                }

                context.Produkt.Add(
                    new WoM_Labb1.Models.Produkt
                    {
                        productName = "Fotboll",
                        productDescription = "Känn dig som Zlatan med den här suveräna bollen",
                        productInStore = 10,
                        productPrice = 149,
                        productImage = bytes
                    });

                file = new FileInfo(@".\wwwroot\Images\tennisboll.jpg");
                using (BinaryReader reader = new BinaryReader(file.OpenRead()))
                {
                    bytes = reader.ReadBytes(Convert.ToInt32(file.Length));
                }

                context.Produkt.Add(
                    new WoM_Labb1.Models.Produkt
                    {
                        productName = "Tennisboll",
                        productDescription = "En boll som man kan spela tennis med!",
                        productInStore = 14,
                        productPrice = 29,
                        productImage = bytes
                    });


                file = new FileInfo(@".\wwwroot\Images\volleyboll.jpg");
                using (BinaryReader reader = new BinaryReader(file.OpenRead()))
                {
                    bytes = reader.ReadBytes(Convert.ToInt32(file.Length));
                }

                context.Produkt.Add(
                    new WoM_Labb1.Models.Produkt
                    {
                        productName = "Volleyboll",
                        productDescription = "En boll som man kan spela volleyboll med!",
                        productInStore = 12,
                        productPrice = 179,
                        productImage = bytes
                    });


                file = new FileInfo(@".\wwwroot\Images\handboll.jpg");
                using (BinaryReader reader = new BinaryReader(file.OpenRead()))
                {
                    bytes = reader.ReadBytes(Convert.ToInt32(file.Length));
                }

                context.Produkt.Add(
                    new WoM_Labb1.Models.Produkt
                    {
                        productName = "Handboll",
                        productDescription = "En boll som man kan spela handboll med!",
                        productInStore = 4,
                        productPrice = 99,
                        productImage = bytes
                    });


                file = new FileInfo(@".\wwwroot\Images\basketboll.jpg");
                using (BinaryReader reader = new BinaryReader(file.OpenRead()))
                {
                    bytes = reader.ReadBytes(Convert.ToInt32(file.Length));
                }

                context.Produkt.Add(
                    new WoM_Labb1.Models.Produkt
                    {
                        productName = "Basketboll",
                        productDescription = "En boll som man kan spela basket med!",
                        productInStore = 7,
                        productPrice = 199,
                        productImage = bytes
                    });

                context.SaveChanges();
            }
        }
    }
}
