using Cldv6212_RetailAppPartThree.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cldv6212_RetailAppPartThree.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddToCart(Product p, int quantity = 1)
        {
            var cart = SessionExtensions.GetJson<List<CartItem>>(HttpContext.Session, "cart") ?? new List<CartItem>();
            var productInCart = cart.FirstOrDefault(c => c.ProductId == p.ProductId);
            if (productInCart != null)
            {

                productInCart.Quantity += quantity;
            }
            else
            {

                cart.Add(new CartItem
                {
                    ProductId = p.ProductId,
                    Description = p.Description,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Quantity = Math.Max(1, quantity)
                });
            }

            SessionExtensions.SetJson(HttpContext.Session, "cart", cart);

            TempData["Success"] = "Product successfully added to cart!";
            return RedirectToAction("UserIndex", "Products");
        }
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = SessionExtensions.GetJson<List<CartItem>>(HttpContext.Session, "cart") ?? new List<CartItem>();

            var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
            }

            SessionExtensions.SetJson(HttpContext.Session, "cart", cart);

            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("cart");
            TempData["Success"] = "Cart cleared.";
            return RedirectToAction("ViewCart");
        }


        [HttpGet]
        public IActionResult ViewCart()
        {
            var cart = SessionExtensions.GetJson<List<CartItem>>(HttpContext.Session, "cart") ?? new List<CartItem>();
            ViewBag.itemCount = cart.Sum(c => c.Quantity);

            return View(cart);
        }
    }
}
public static class SessionExtensions
{
    public static readonly System.Text.Json.JsonSerializerOptions jsonOptions = new(System.Text.Json.JsonSerializerOptions.Default)
    {
        PropertyNameCaseInsensitive = true
    };
    public static void SetJson(this ISession session, string key, object value)
    {
        session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value, jsonOptions));
    }

    public static T? GetJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default(T) : System.Text.Json.JsonSerializer.Deserialize<T>(value, jsonOptions);
    }
}