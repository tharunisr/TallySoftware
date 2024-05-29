using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallySoftware.DTO;
using TallySoftware.Entity;
using TallySoftware.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TallySoftware.Controllers
{

    public class CustomerController : Controller
    {
        private string? customertype = null;
        private string? search = null;

        private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpGet]
        public ActionResult AddCustomer()
        {
            List<string> CustomerTypeNames = new List<string>();
            CustomerTypeNames = GetCustomerTypeName().Result;
            if (CustomerTypeNames.Count > 0)
            {
                ViewBag.customerType = CustomerTypeNames;
            }

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
                    Customer customer = new Customer();
                    customer = MapEntity(customer, customerDTO);

                    await _context.AddAsync(customer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("DisplayCustomer");
                    //if (HttpContext.Session.GetString("LoggedInUserType") != null)
                    //{
                    //    String UserType = HttpContext.Session.GetString("LoggedInUserType");
                    //    if (UserType == "Admin")
                    //    {
                    //        return RedirectToAction("Admindashboard", "staff");
                    //    }
                    //    else
                    //    {
                    //        return RedirectToAction("staffdashboard", "staff");
                    //    }

                    //}
                }
            }
            return RedirectToAction("AddCustomer");
        }
        [HttpGet]
        public async Task<ActionResult> DisplayCustomer(string customertype = null, string search = null,int pageno=1)
        {
            this.customertype = customertype;
            this.search = search;
            var query = _context.Customers.Where(c => !c.IsDeleted).AsQueryable();
            List<string> customertypeName = new List<string>();
            customertypeName = GetCustomerTypeName().Result;
            if (customertypeName.Count > 0)
            {
                ViewBag.customertypeName = customertypeName;
            }
            if (!string.IsNullOrWhiteSpace(customertype) && customertype != "All")
            {
                query = query.Where(c => c.CustomerTypeName.Equals(customertype));
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Name.Equals(search) || c.PhoneNumber.Equals(search));
            }
            List<Customer> Customer = await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();
            Customer = query.ToList();
            ViewBag.TotalCount = Customer.Count;
            int noofrecordperpage = 10;
            int noofpage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Customer.Count) / Convert.ToDouble(noofrecordperpage)));
            int noofrecordstoskip = (pageno - 1) * noofrecordperpage;
            ViewBag.pageno = pageno;
            ViewBag.noofpage = noofpage;
            Customer = Customer.Skip(noofrecordstoskip).Take(noofrecordperpage).ToList();

            return View(Customer);
        }

        [HttpGet]
        public ActionResult ClearFilters()
        {
            this.customertype = "All";
            this.search = null;
            return RedirectToAction("DisplayCustomer");
        }
        public async Task<List<String>> GetCustomerTypeName()
        {

            List<string> customerType = await _context.CustomerTypes.Select(s => s.CustomerTypeName).ToListAsync();
            return customerType;
        }
        public Customer MapEntity(Customer customer,CustomerDTO customerDTO)
        {
            
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

        [HttpGet]
        public async Task<ActionResult> EditCustomer(int id)
        {
            List<string> CustomerTypeNames = new List<string>();
            CustomerTypeNames = GetCustomerTypeName().Result;
            if (CustomerTypeNames.Count > 0)
            {
                ViewBag.customerType = CustomerTypeNames;
            }
            CustomerDTO customerDTO = new CustomerDTO();
            Customer customer = GetCustomerById(id).Result;
            
            customerDTO.Customerid = customer.CustomerId;
            customerDTO.Name = customer.Name;
            customerDTO.Address=customer.Address;
            customerDTO.PhoneNumber = customer.PhoneNumber;
            customerDTO.CompanyName=customer.CompanyName;
            customerDTO.CustomerTypeName=customer.CustomerTypeName;
            customerDTO.Remark=customer.Remark;
            customerDTO.AdministrativeId= customer.AdministrativeId;
            customerDTO.ContactPersonName=customer.ContactPersonName;
            return View(customerDTO);
        }
        [HttpPost]
        public async Task<ActionResult> EditCustomer(CustomerDTO customerDTO)
        {
            if (customerDTO != null && ModelState.IsValid)
            {
                Customer customer = new Customer();
                 customer = GetCustomerById(customerDTO.Customerid).Result;
                 customer =   MapEntity(customer,customerDTO);
                customer.UpdatedBy = HttpContext.Session.GetString("LoggedInUserName");
                customer.UpdatedOn = DateTime.Now;
                _context.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("DisplayCustomer");
            }
            else
            {
                ViewBag.error = "Please enter required fields";
                return View();
            }
        }
        [HttpGet]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            Customer customer = GetCustomerById(id).Result;
            customer.IsDeleted = true;
            customer.UpdatedOn = DateTime.Now;
            customer.UpdatedBy = HttpContext.Session.GetString("LoggedInUserName");
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction("DisplayCustomer");

        }
            public async Task<Customer> GetCustomerById(int customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(e => e.CustomerId == customerId);
            return customer;
        }
    }
}

