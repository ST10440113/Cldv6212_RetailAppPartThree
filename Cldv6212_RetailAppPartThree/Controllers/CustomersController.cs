using Cldv6212_RetailAppPartThree.Models;
using Cldv6212_RetailAppPartThree.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cldv6212_RetailAppPartThree.Controllers
{
    public class CustomersController : Controller
    {

        private readonly TableService _svc;
        private readonly QueueService _queueService;


        public CustomersController(TableService table, QueueService queueService)
        {

            _svc = table;
            _queueService = queueService;
        }

        // GET: Customers

        public async Task<IActionResult> Index()
        {
            var customers = await _svc.ListCustomersAsync();
            return View(customers);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            var details = await _svc.GetCustomerAsync(partitionKey, rowKey);
            return View(details);
        }

        // GET: Customers/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FirstName,LastName,Email,PhoneNumber,Address")] Customer customer)
        {

            customer.CustomerId = customer.GetHashCode();
            customer.PartitionKey = "CUSTOMER";
            customer.RowKey = Guid.NewGuid().ToString("N");


            await _svc.AddCustomerAsync(customer);

            //message queue insert
            string message = $"New customer added: Customer First Name: {customer.FirstName}," +
                $" Customer Last Name {customer.LastName}," +
                $" Email {customer.Email} , " +
                $" Phone Number {customer.PhoneNumber} , " +
                $" Address {customer.Address}  ";


            await _queueService.SendMessageAsync(message);

            return RedirectToAction(nameof(Index));

        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                return NotFound();
            }
            var oldCustomer = await _svc.GetCustomerAsync(partitionKey, rowKey);

            if (oldCustomer == null)
            {
                return NotFound();
            }

            return View(oldCustomer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CustomerId,FirstName,LastName,Email,PhoneNumber,Address,PartitionKey,RowKey")] Customer customer)
        {

            if (customer == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _svc.UpdateCustomerAsync(customer);



                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            await _svc.DeleteCustomerAsync(partitionKey, rowKey);
            return RedirectToAction(nameof(Index));
        }


    }
}
