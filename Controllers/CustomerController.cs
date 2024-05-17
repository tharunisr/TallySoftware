
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallySoftware.DTO;
using TallySoftware.Entity;
using TallySoftware.Services;

namespace TallySoftware.Controllers
{
    
        public class CustomerController : Controller
        {
            
            private readonly ApplicationDbContext _context;
            public CustomerController(ApplicationDbContext context)
            {
                _context = context;
                
            }
            [HttpGet]
            public ActionResult AddCustomer()
            {
                return View();
            }
            [HttpPost]
            public async Task<ActionResult> AddCustomer(CustomerDTO customerDTO)
            {
                if (customerDTO == null)
                {
                    ViewBag.Error = "Invalid Data";
                }
                else
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.Error = "Please enter the required fields";
                    }
                    else
                    {
                        Customer customer = MapEntity(customerDTO);
                        await _context.AddAsync(customer);
                        await _context.SaveChangesAsync();
                    if(HttpContext.Session.GetString("LoggedInUserType") != null)
                        {
                        String UserType = HttpContext.Session.GetString("LoggedInUserType");
                        if (UserType == "Admin")
                        {
                            return RedirectToAction("Admindashboard","staff");
                        }
                        else
                        {
                            return RedirectToAction("staffdashboard", "staff");
                        }

                    }

}

                }
                return View();
            }
        [HttpGet]
        public async Task<ActionResult> DisplayCustomer(Customer customer)
        {
            List<Customer> Customer = await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();
            return View(Customer);
        }
        public Customer MapEntity(CustomerDTO customerDTO)
            {
                Customer customer = new Customer();
                customer.Name = customerDTO.Name;
                customer.Address = !string.IsNullOrEmpty(customerDTO.Address) ? customerDTO.Address : null;
                customer.PhoneNumber = !string.IsNullOrEmpty(customerDTO.PhoneNumber) ? customerDTO.PhoneNumber : null;
            customer.AdministrativeId = customerDTO.AdministrativeId != null ? customerDTO.AdministrativeId : null;
            customer.Remark = !string.IsNullOrEmpty(customerDTO.Remark) ? customerDTO.Remark : null;
                customer.CompanyName = !string.IsNullOrEmpty(customerDTO.CompanyName) ? customerDTO.CompanyName : null;
                customer.ContactPersonName = !string.IsNullOrEmpty(customerDTO.ContactPersonName) ? customerDTO.ContactPersonName : null;
                customer.CustomerTypeName = !string.IsNullOrEmpty(customerDTO.CustomerTypeName) ? customerDTO.CustomerTypeName : null;
                customer.CustomerTypeId = customerDTO.CustomerTypeId != null ? customerDTO.CustomerTypeId : null;
                return customer;
            }
        }
    }

