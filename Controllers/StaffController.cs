using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TallySoftware.DTO;
using TallySoftware.Entity;
using TallySoftware.Models;
using TallySoftware.Services;

namespace TallySoftware.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly ICustomerService _customerService;
        private readonly ApplicationDbContext _context;
        public StaffController(ApplicationDbContext context, IStaffService staffService, ICustomerService customerService)
        {
            _context = context;
            _staffService = staffService;
            _customerService = customerService;
        }
        [HttpGet]
        public async Task<ActionResult> AdminDashboard(string status = null,string recruitment=null,string search=null)
        {
            //List<Customer> customer=_customerService.GetCustomers().Result;
            List<EnquiryEntity> enquiries = new List<EnquiryEntity>();
            //List<EnquiryEntity> enquiries = await _context.Enquiries.Include(c => c.Customer)
            //    .Where(c => !c.IsDeleted)
            //    .ToListAsync();
            var query = _context.Enquiries.Include(c => c.Customer)
                .AsQueryable();
           
            if (!string.IsNullOrWhiteSpace(status) && status!="All")
            {
                query = query.Where(c => c.Status.Equals(status));
            }
            if (!string.IsNullOrWhiteSpace(recruitment) && recruitment != "All")
            {
                query = query.Where(c => c.Recruitment.Equals(recruitment));
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.CustomerName.Equals(search)||c.Customer.PhoneNumber.Equals(search));
            }

            enquiries = query.ToList();

            return View(enquiries);
        }
        [HttpGet]
        public ActionResult AddStaff()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddStaff(Staff staff)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(staff.StaffName) && !string.IsNullOrWhiteSpace(staff.Password))
                {
                    Staff user = _staffService.GetStaff(staff.StaffName).Result;
                    if (user != null)
                    {
                        ViewBag.Error = "Staff already exist with name";
                    }
                    else
                    {
                        staff.StaffType = "Staff";
                        await _context.Staffs.AddAsync(staff);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("AdminDashboard");

                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }
        [HttpGet]
        public ActionResult AddCustomer()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddCustomer(Staff staff)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(staff.StaffName) && !string.IsNullOrWhiteSpace(staff.Password))
                {
                    Staff user = _staffService.GetStaff(staff.StaffName).Result;
                    if (user != null)
                    {
                        ViewBag.Error = "Staff already exist with name";
                    }
                    else
                    {
                        staff.StaffType = "Staff";
                        await _context.Staffs.AddAsync(staff);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("AdminDashboard");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }
        [HttpGet]
        public ActionResult Enquiry()
        {
            EnquiryDTO enquiryDTO = new EnquiryDTO();
            List<string> CustomerNames = new List<string>();
            CustomerNames = _customerService.GetCustomersName().Result;
            //ViewBag.customer = CustomerNames;
            if (CustomerNames.Count > 0)
            {
                enquiryDTO.CustomerNameList = CustomerNames;
                ViewBag.CustomerNameList = enquiryDTO.CustomerNameList;
            }
            return View(enquiryDTO);
        }
        [HttpPost]
        public async Task<ActionResult> Enquiry(EnquiryDTO enquiryEntity)
        {
            if (enquiryEntity == null)
            {
                ViewBag.Error = "Invalid Data";
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Error = "Please enter the required fields";
                    return RedirectToAction("enquiry");
                }
                else
                {
                    //enquiryEntity
                    EnquiryEntity enquiry = MapEntity(enquiryEntity);
                    if (enquiry != null)
                    {
                        Customer customer = _customerService.GetCustomerByName(enquiryEntity.CustomerName).Result;
                        enquiry.CustomerId = customer.CustomerId;
                        enquiry.CreatedOn = DateTime.Now;
                        enquiry.CreatedBy = HttpContext.Session.GetString("LoggedInUserName");
                        await _context.AddAsync(enquiry);
                        await _context.SaveChangesAsync();
                        if (HttpContext.Session.GetString("LoggedInUserType") != null)
                        {
                            String UserType = HttpContext.Session.GetString("LoggedInUserType");
                            if (UserType == "Admin")
                            {
                                return RedirectToAction("Admindashboard", "staff");
                            }
                            else
                            {
                                return RedirectToAction("staffdashboard", "staff");
                            }

                        }
                    }
                }
            }
            return View();
        }
     
        public EnquiryEntity MapEntity(EnquiryDTO enquiryDTO)
        {
            EnquiryEntity enquiry = new EnquiryEntity();
            enquiry.CustomerName = enquiryDTO.CustomerName;
            enquiry.Status = enquiryDTO.Status;
            enquiry.Recruitment=enquiryDTO.RecruitmentName;
            enquiry.Remark = enquiryDTO.Remark;
            enquiry.Schedule = enquiryDTO.Schedule;
            enquiry.Payment=enquiryDTO.Payment;
            return enquiry;
        }
    }
}