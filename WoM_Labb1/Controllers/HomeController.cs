using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WoM_Labb1.Models;
using WoM_Labb1.Data;

namespace WoM_Labb1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProduktContext _context;

        public HomeController(ILogger<HomeController> logger, ProduktContext context)
        {
            this._context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_context.Produkt);
        }

        /// <summary>
        /// Retreives a specific product from database and sends it to view.
        /// </summary>
        /// <param name="item">The ID of the product to be displayed</param>
        public IActionResult Product(int item)
        {
            IList<Produkt> produktList = new List<Produkt>();

            var result = from itemProdukt in _context.Produkt
                         where itemProdukt.Id == item
                         select itemProdukt;

            foreach (var produktItem in result)
                produktList.Add(produktItem);

            ViewData["Message"] = produktList;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Passes shoppingcart, recommendationlist and sum for every product to view.
        /// </summary>
        /// <param name="errorMessage">Optional error message to be displayed in view</param>
        public IActionResult Varukorg(string errorMessage=null)
        {
            IList<Produkt> produktList = new List<Produkt>();
            IList<ShoppingCart> produktsInCart = new List<ShoppingCart>();
            IList<int> sum = new List<int>();
            int totalsum = 0;

            totalsum = SumCalc(produktsInCart, sum, totalsum);
            RecommendList(produktList);

            ViewData["errorMessage"] = errorMessage;
            ViewData["produktsInCart"] = produktsInCart;
            ViewData["sum"] = sum;
            ViewData["totalSum"] = totalsum;
            ViewData["totalSumVAT"] = (totalsum * 1.25);
            ViewData["produktList"] = produktList;
            return View(_context);
        }

        /// <summary>
        /// Creates a list with product recommendations based on other previously bought products
        /// </summary>
        /// <param name="produktList">The list that shall be filled with product recommendations</param>
        private void RecommendList(IList<Produkt> produktList)
        {
            int displaySlots = 5;

            //Code below is most likely more complex than it has to be.
            //If a product from existing shoppingcart also exists in another order 
            //then up to 5 other items from the same order will be added into recommendation list.
            foreach (var item in _context.ShoppingCart)
            {
                var itemResult = from orderdetails in _context.OrderDetails
                                 where item.Id == orderdetails.Id
                                 select orderdetails;
                foreach (var orderDetailItem in itemResult)
                {
                    var result = from orderdetails in _context.OrderDetails
                                 where orderDetailItem.orderId == orderdetails.orderId
                                 select orderdetails.Id;

                    foreach (var resultItem in result)
                    {
                        var produktResult = from produkt in _context.Produkt
                                            where produkt.Id == resultItem
                                            select produkt;
                        foreach (var produkt in produktResult)
                        {
                            var redundantCheck = from shoppingcart in _context.ShoppingCart
                                                 where shoppingcart.Id == produkt.Id
                                                 select shoppingcart;

                            //Only adds a product to list if same product does not already exist in list or shoppingcart.
                            //Only adds up to 5 products to list
                            if (redundantCheck.Count<ShoppingCart>() == 0 && !produktList.Contains(produkt) && displaySlots != 0)
                            {
                                produktList.Add(produkt);
                                displaySlots--;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the total price of every product in the shoppingcart
        /// </summary>
        /// <param name="produktsInCart">Empty list to be filled with all items from shoppingcart</param>
        /// <param name="sum">Cost for each product</param>
        /// <param name="totalsum">Total cost</param>
        /// <returns>Total cost of all items</returns>
        private int SumCalc(IList<ShoppingCart> produktsInCart, IList<int> sum, int totalsum)
        {
            foreach (var item in _context.ShoppingCart)
            {
                var itemResult = from produkt in _context.Produkt
                                 where item.Id == produkt.Id
                                 select produkt;

                produktsInCart.Add(item);

                foreach (var produktItem in itemResult)
                {
                    sum.Add(item.Antal * produktItem.productPrice);
                    totalsum += (item.Antal * produktItem.productPrice);
                }
            }

            return totalsum;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Changes the quantity of a product in shoppingcart. 
        /// </summary>
        /// <param name="itemName">Name of the product</param>
        /// <param name="itemAmount">Quantity of the product</param>
        public IActionResult EditItem(string itemName, int itemAmount)
        {
            var result = from item in _context.Produkt
                         where item.productName == itemName
                         select item;

            foreach (var item in result)
            {
                if (itemAmount < 1 || itemAmount > item.productInStore)
                {
                    return RedirectToAction("Varukorg");
                }

                foreach (var cartItem in _context.ShoppingCart)
                {
                    if (cartItem.Id == item.Id)
                        cartItem.Antal = itemAmount;
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Varukorg");
        }

        /// <summary>
        /// Adds a product to the shoppingcart
        /// </summary>
        /// <param name="itemName">The name of the product</param>
        /// <param name="itemAmount">Quantity of the product</param>
        public IActionResult AddItem(string itemName, int itemAmount)
        {
            int itemId = 1;
            bool inCart = false;
            var result = from item in _context.Produkt
                         where item.productName == itemName
                         select item;

            foreach (var item in result)
            {
                if (itemAmount < 1 || item.productInStore < 1 || itemAmount > item.productInStore)
                {
                    return RedirectToAction("Product", new { item = item.Id });
                }

                WhenAlreadyInCart(itemAmount, ref itemId, ref inCart, item);

                if (!inCart)
                {
                    itemId = WhenNotInCart(itemAmount, item);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Product", new { item = itemId });
        }

        /// <summary>
        /// If item already exists in cart, add itemAmount instead of set itemAmount.
        /// </summary>
        /// <param name="itemAmount">Quantity</param>
        /// <param name="itemId">The ID of the product</param>
        /// <param name="inCart">Flagged as true when product already exists in cart </param>
        /// <param name="item">The product</param>
        private void WhenAlreadyInCart(int itemAmount, ref int itemId, ref bool inCart, Produkt item)
        {
            foreach (var cartItem in _context.ShoppingCart)
            {
                if (cartItem.Id == item.Id)
                {
                    itemId = item.Id;
                    inCart = true;
                    cartItem.Antal += itemAmount;
                    if (cartItem.Antal > item.productInStore)
                        cartItem.Antal = item.productInStore;
                }
            }
        }

        /// <summary>
        /// Add the product to shoppingcart.
        /// </summary>
        /// <param name="itemAmount">Quantity of the product</param>
        /// <param name="item">Product to be added to cart</param>
        /// <returns>ID of the product</returns>
        private int WhenNotInCart(int itemAmount, Produkt item)
        {
            int itemId;

            _context.ShoppingCart.Add(
            new ShoppingCart
            {
                Id = item.Id,
                Antal = itemAmount,
                Produkt = item
            });
            itemId = item.Id;

            return itemId;
        }

        public IActionResult Search()
        {
            return View();
        }

        /// <summary>
        /// Searches for an order and displays the orderdetails
        /// </summary>
        /// <param name="searchId">Order ID</param>
        public IActionResult SearchResult(int searchId)
        {
            IList<Customer> CustomerList = new List<Customer>();
            IList<Order> OrderList = new List<Order>();
            IList<OrderDetails> OrderDetailsList = new List<OrderDetails>();
            IList<Produkt> ProduktList = new List<Produkt>();
            int totalsum = 0;
            double totalsumVAT = 0;


            var result = from order in _context.Order
                         where order.orderId == searchId
                         select order;

            //Searches for all the needed information from the order and sends it to View
            foreach(var item in result)
            {
                OrderList.Add(item);
                var itemResult = from customer in _context.Customer
                                 where customer.personnummer == item.personnummer
                                 select customer;
                foreach (var customer in itemResult)
                    CustomerList.Add(customer);

                var orderIdResult = from orderDetails in _context.OrderDetails
                                    where item.orderId == orderDetails.orderId
                                    select orderDetails;
                foreach(var orderId in orderIdResult)
                {
                    OrderDetailsList.Add(orderId);
                    var produktResult = from produkt in _context.Produkt
                                       where produkt.Id == orderId.Id
                                       select produkt;
                    foreach (var produktItem in produktResult)
                    {
                        totalsum += orderId.Antal * orderId.Pris;
                        ProduktList.Add(produktItem);
                    }
                }
            }

            totalsumVAT = totalsum * 1.25;

            ViewData["SumVAT"] = totalsumVAT;
            ViewData["Sum"] = totalsum;
            ViewData["Produkt"] = ProduktList;
            ViewData["OrderDetail"] = OrderDetailsList;
            ViewData["Customer"] = CustomerList;
            ViewData["Order"] = OrderList;
            return View();
        }
    }
}
