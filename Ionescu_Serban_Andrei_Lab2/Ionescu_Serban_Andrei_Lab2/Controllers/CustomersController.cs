using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ionescu_Serban_Andrei_Lab2.Data;
using Ionescu_Serban_Andrei_Lab2.Models;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Ionescu_Serban_Andrei_Lab2.Controllers
{
    [Authorize(Policy = "SalesManager")]
    public class CustomersController : Controller
    {
        private readonly LibraryContext _context;
        private string _baseUrl = "http://localhost:5182/api/Customers";
        public CustomersController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(_baseUrl);
            if (response.IsSuccessStatusCode)
            {
                var customers = JsonConvert.DeserializeObject<List<Customer>>(await
                response.Content.
                ReadAsStringAsync());
                return View(customers);
            }
            return NotFound();
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");
            if (response.IsSuccessStatusCode)
            {
                var customer = await _context.Customers.AsNoTracking().Include(b=>b.City)
                    .FirstOrDefaultAsync(m => m.CustomerID == id);
                return View(customer);
            }
            return NotFound();
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "CityName");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,Name,Adress,BirthDate,CityID")] Customer customer)
        {
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "CityName");
            if (!ModelState.IsValid) return View(customer);
            try
            {
                var client = new HttpClient();
                string json = JsonConvert.SerializeObject(customer);
                var response = await client.PostAsync(_baseUrl,
                new StringContent(json, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to create record:{ ex.Message}");
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "CityName");
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var customerToUpdate = await _context.Customers.FirstOrDefaultAsync(s => s.CustomerID == id);
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "CityName");

            if (await TryUpdateModelAsync<Customer>(
                customerToUpdate,
                "",
                s => s.Name, s => s.Adress, s => s.BirthDate, s => s.City))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists");
                }
            }
            return View(customerToUpdate);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");
            if (response.IsSuccessStatusCode)
            {
                var customer = await _context.Customers.Include(b =>b.City).AsNoTracking()
                    .FirstOrDefaultAsync(m=>m.CustomerID == id);
                return View(customer);
            }
            return new NotFoundResult();
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("CustomerID")] Customer customer)
        {
            try
            {
                var client = new HttpClient();
                HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Delete,
                $"{_baseUrl}/{customer.CustomerID}")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(customer),
                Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(request);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to delete record:{ ex.Message}");
            }
            return View(customer);
        }

        private bool CustomerExists(int id)
        {
          return (_context.Customers?.Any(e => e.CustomerID == id)).GetValueOrDefault();
        }
    }
}
