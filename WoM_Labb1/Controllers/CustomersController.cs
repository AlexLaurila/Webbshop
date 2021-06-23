using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WoM_Labb1.Data;
using WoM_Labb1.Models;

namespace WoM_Labb1.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ProduktContext _context;

        public CustomersController(ProduktContext context)
        {
            _context = context;
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("personnummer,fornamn,efternamn,postadress,postnr,ort,epost,telefonnummer")] Customer customer)
        {
            if (ModelState.IsValid)
            {

                //Final check if order shall be accepted before adding customer to database.
                var result = from item in _context.Customer
                             where item.personnummer == customer.personnummer
                             select item;

                string notInStockCheck = IfNotInStock();
                if (notInStockCheck != null)
                {
                    return RedirectToAction("Varukorg", "Home", new { errorMessage = notInStockCheck });
                }

                //Adds user to database if it doesnt already exist. Redirects user to Checkout.
                foreach (var item in result)
                {
                    return RedirectToAction("Checkout", customer);
                }

                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Checkout", customer);
            }
            return View(customer);
        }

        /// <summary>
        /// Creates a new order and extracts the information from shoppingcart
        /// </summary>
        /// <param name="customer">Customer that owns the order</param>
        public IActionResult Checkout(Customer customer)
        {
            Order tempOrder = new Order()
            {
                personnummer = customer.personnummer
            };
            _context.Order.Add(tempOrder);

            List<OrderDetails> orderDetailLista = new List<OrderDetails>();
            FillOrderDetailsList_ThenRemoveCart(tempOrder, orderDetailLista);
            tempOrder.OrderDetails = orderDetailLista;

            _context.SaveChanges();

            ViewData["Message"] = tempOrder.orderId;
            return View();
        }

        /// <summary>
        /// Function fills OrderDetails list with information about quantity, the product itself and ID. Removes the shoppingcart when done
        /// </summary>
        /// <param name="tempOrder"></param>
        /// <param name="orderDetailLista"></param>
        private void FillOrderDetailsList_ThenRemoveCart(Order tempOrder, List<OrderDetails> orderDetailLista)
        {
            foreach (var item in _context.ShoppingCart)
            {
                foreach (var item1 in _context.Produkt)
                {
                    if (item.Id == item1.Id)
                    {
                        item1.productInStore -= item.Antal;

                        OrderDetails tempOrderDetails = new OrderDetails()
                        {
                            Antal = item.Antal,
                            Produkt = item.Produkt,
                            orderId = tempOrder.orderId,
                            Id = item1.Id,
                            Pris = item1.productPrice
                        };

                        _context.OrderDetails.Add(tempOrderDetails);
                        orderDetailLista.Add(tempOrderDetails);
                        _context.ShoppingCart.Remove(item);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the product has enough in stock to create order or not. If not enough in stock then function shows error message
        /// </summary>
        public string IfNotInStock()
        {
            foreach (var item in _context.ShoppingCart)
            {
                var result = from itemProdukt in _context.Produkt
                             where item.Id == itemProdukt.Id && item.Antal > itemProdukt.productInStore
                             select itemProdukt;

                foreach(var produktItem in result)
                {
                    return $"Finns inte tillräckligt många {produktItem.productName} i lager. Var vänlig justera antalet och försök igen";
                }
            }
            return null;
        }
    }
}
