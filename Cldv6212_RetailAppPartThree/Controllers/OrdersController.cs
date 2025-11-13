using Cldv6212_RetailAppPartThree.Models;
using Cldv6212_RetailAppPartThree.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cldv6212_RetailAppPartThree.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TableService _svc;
        private readonly QueueService _queueService;


        public OrdersController(QueueService queue, TableService table)
        {
            _queueService = queue;
            _svc = table;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _svc.ListOrdersAsync();
            var customers = await _svc.ListCustomersAsync();
            var products = await _svc.ListProductsAsync();


            foreach (var order in orders)
            {
                order.Customer = customers.FirstOrDefault(c => c.CustomerId == order.CustomerId);
                order.Product = products.FirstOrDefault(p => p.ProductId == order.ProductId);

            }

            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                return NotFound();
            }

            var order = await _svc.GetOrderAsync(partitionKey, rowKey);
            var customers = await _svc.ListCustomersAsync();
            var products = await _svc.ListProductsAsync();


            order.Customer = customers.FirstOrDefault(c => c.CustomerId == order.CustomerId);
            order.Product = products.FirstOrDefault(p => p.ProductId == order.ProductId);


            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {

            var customers = await _svc.ListCustomersAsync();
            var products = await _svc.ListProductsAsync();


            //check for empty customers
            if (customers == null || customers.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No customers available. Please add a customer before creating an order.");
                return View();
            }


            //check for empty products
            if (products == null || products.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No products available. Please add a product before creating an order.");
                return View();
            }
            ViewData["Customer"] = customers;
            ViewData["Product"] = products;
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,Status,CustomerId,ProductId,PartitionKey,RowKey")] Order order)
        {
            //table storage insert
            order.OrderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);
            order.PartitionKey = "ORDER";
            order.RowKey = Guid.NewGuid().ToString("N");
            await _svc.AddOrderAsync(order);

            //message queue insert
            string message = "New order placed!";


            await _queueService.SendMessageAsync(message);
            return RedirectToAction(nameof(Index));

        }


        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            var customers = await _svc.ListCustomersAsync();
            var products = await _svc.ListProductsAsync();
            var order = await _svc.GetOrderAsync(partitionKey, rowKey);

            order.Customer = customers.FirstOrDefault(c => c.CustomerId == order.CustomerId);
            order.Product = products.FirstOrDefault(p => p.ProductId == order.ProductId);


            ViewData["Customer"] = customers;
            ViewData["Product"] = products;


            return View(order);
        }
        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string partitionKey, string rowKey, [Bind("OrderId,OrderDate,Status,CustomerId,ProductId, PartitionKey, RowKey")] Order order)
        {

            order.OrderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);
            partitionKey = order.PartitionKey;
            rowKey = order.RowKey;


            await _svc.UpdateOrderAsync(order);

            return RedirectToAction(nameof(Index));


        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            //get order to delete
            var o = await _svc.GetOrderAsync(partitionKey, rowKey);

            if (o == null)
            {
                return NotFound();
            }

            if (o != null)
            {
                await _svc.DeleteOrderAsync(partitionKey, rowKey);
            }


            return RedirectToAction(nameof(Index));
        }

    }
}
