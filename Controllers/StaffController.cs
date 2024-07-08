using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TallySoftware.DTO;
using TallySoftware.Entity;
using TallySoftware.Models;
using TallySoftware.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TallySoftware.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly ICustomerService _customerService;
        private readonly ApplicationDbContext _context;
        private string? status = null;
        private string? recruitment = null;
        private string? search = null;
        public StaffController(ApplicationDbContext context, IStaffService staffService, ICustomerService customerService)
        {
            _context = context;
            _staffService = staffService;
            _customerService = customerService;
        }
        public async Task<ActionResult> DashBoard()
        {
            EnquiryStatusCountModel statuscount = new EnquiryStatusCountModel();
            var enquiry = _context.Enquiries.Include(e => e.Customer)
                .Where(e => !e.IsDeleted && !e.Customer.IsDeleted).AsQueryable();
            statuscount.Lead = enquiry
                .Where(e => e.Status.Equals("Lead") || e.Status.Equals("Processing") || e.Status.Equals("New Customer")
                || e.Status.Equals("Order Confirmed") || e.Status.Equals("Qualified")).Count();
            statuscount.PaymentPending = enquiry
                .Where(e => e.Status.Equals("Payment Pending")).Count();
            statuscount.Completed = enquiry
                .Where(e => e.Status.Equals("Completed")).Count();
            statuscount.Rejected = enquiry
                .Where(e => e.Status.Equals("Rejected")).Count();
            statuscount.New = enquiry
                .Where(e => e.Status.Equals("New")).Count();
            statuscount.TodaySchedule = enquiry
               .Where(e => e.Schedule.Day.Equals(DateTime.Now.Day)
               && e.Schedule.Month.Equals(DateTime.Now.Month) && e.Schedule.Year.Equals(DateTime.Now.Year)).Count();
            return View(statuscount);
        }

        [HttpGet]
        public async Task<ActionResult> AdminDashboard(DateTime? schedule = null, string status = null,
            string recruitment = null, string search = null, int pageno = 1)
        {

            ViewBag.usertype = HttpContext.Session.GetString("LoggedInUserType");
            if (!string.IsNullOrWhiteSpace(status))
            {
                HttpContext.Session.SetString("status", status);
            }
            //if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("status")) && !schedule.HasValue)
            //{
            //    status = HttpContext.Session.GetString("status");
            //}

            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("status")))
            {
                status = HttpContext.Session.GetString("status");
            }

            this.status = status;
            ViewBag.status = status;
            this.recruitment = recruitment;
            ViewBag.recruitment = recruitment;
            this.search = search;
            ViewBag.schedule = schedule != null ? DateOnly.FromDateTime(schedule.Value).ToString("yyyy-MM-dd")
                : null;
            ViewBag.search = !String.IsNullOrEmpty(search) ? search : null;
            List<EnquiryEntity> enquiries = new List<EnquiryEntity>();
            var query = _context.Enquiries.Include(c => c.Customer)
               .Where(e => !e.IsDeleted && !e.Customer.IsDeleted).AsQueryable();
            List<string> RecruitmentNames = new List<string>();
            RecruitmentNames = GetRecruitmentName().Result;
            List<string> StatusNames = new List<string>();
            if (schedule.HasValue)
            {
                StatusNames = GetStatusName().Result;
            }
            else
            {
                StatusNames = GetStatusNameForDashboard().Result;
            }
            if (StatusNames.Count > 0)
            {
                ViewBag.statusType = StatusNames;
            }
            if (RecruitmentNames.Count > 0)
            {
                ViewBag.recruitmentType = RecruitmentNames;
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "Lead")
                {
                    query = query.Where(c => c.Status.Equals("Lead") || c.Status.Equals("New Customer")
                || c.Status.Equals("Processing") || c.Status.Equals("Qualified") || c.Status.Equals("Order Confirmed"));
                }
                else if (status == "Processing")
                {
                    query = query.Where(c => c.Status.Equals("Processing"));
                }
                else if (status == "Qualified")
                {
                    query = query.Where(c => c.Status.Equals("Qualified"));
                }
                else if (status == "Order Confirmed")
                {
                    query = query.Where(c => c.Status.Equals("Order Confirmed"));
                }
                else if (status == "New Customer")
                {
                    query = query.Where(c => c.Status.Equals("New Customer"));
                }
                else if (status == "Payment Pending")
                {
                    query = query.Where(c => c.Status.Equals("Payment Pending"));
                }
                else if (status == "Completed")
                {
                    query = query.Where(c => c.Status.Equals("Completed"));
                }
                else if (status == "Rejected")
                {
                    query = query.Where(c => c.Status.Equals("Rejected"));
                }
                else if (status == "New")
                {
                    query = query.Where(c => c.Status.Equals("New"));
                }

            }
            if (!string.IsNullOrWhiteSpace(recruitment) && recruitment != "All")
            {
                query = query.Where(c => c.Recruitment.Equals(recruitment));
            }
            if (schedule != null)
            {
                query = query.Where(c => c.Schedule.Day == schedule.Value.Day &&
                c.Schedule.Month == schedule.Value.Month && c.Schedule.Year == schedule.Value.Year);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.CustomerName.Contains(search)
                || c.Customer.PhoneNumber.Contains(search));
            }
            enquiries = query.ToList();
            ViewBag.TotalCount = enquiries.Count;
            int noofrecordperpage = 5;
            int noofpage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(enquiries.Count) / Convert.ToDouble(noofrecordperpage)));
            int noofrecordstoskip = (pageno - 1) * noofrecordperpage;
            ViewBag.pageno = pageno;
            ViewBag.noofpage = noofpage;
            enquiries = enquiries.Skip(noofrecordstoskip).Take(noofrecordperpage).ToList();
            return View(enquiries);
        }

        public async Task<ActionResult> DisplayStaff()
        {
            ViewBag.usertype = HttpContext.Session.GetString("LoggedInUserType");
            var query = _context.Staffs.Where(c => !c.IsDeleted).AsQueryable();
            List<Staff> staff = await _context.Staffs.Where(c => !c.IsDeleted).ToListAsync();
            staff = query.ToList();
            return View(staff);
        }

        [HttpGet]
        public async Task<ActionResult> Deletestaff(int id)
        {
            Staff staff = GetstaffById(id).Result;
            staff.IsDeleted = true;
            _context.Staffs.Update(staff);
            await _context.SaveChangesAsync();
            return RedirectToAction("Displaystaff");
        }

        public async Task<Staff> GetstaffById(int staffId)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(e => e.StaffId == staffId);
            return staff;
        }

        [HttpGet]
        public ActionResult ClearFilters()
        {
            this.status = "All";
            this.recruitment = "All";
            this.search = null;
            return RedirectToAction("AdminDashboard");
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
            enquiryDTO.Schedule = DateTime.Now;
            List<string> staffnames = new List<string>();
            staffnames = GetstaffName().Result;
            List<string> CustomerNames = new List<string>();
            CustomerNames = _customerService.GetCustomersName().Result;
            List<string> StatusNames = new List<string>();
            StatusNames = GetStatusName().Result.Where(s => !s.Equals("All")).ToList() ;
            List<string> RecruitmentNames = new List<string>();
            RecruitmentNames = GetRecruitmentName().Result.Where(s => !s.Equals("All")).ToList();
            if (staffnames.Count > 0)
            {
                ViewBag.resource = staffnames;
            }
            //if (HttpContext.Session.GetString("LoggedInUserType") != null)
            //{
            //    = HttpContext.Session.GetString("LoggedInUserType");
            //}
            if (CustomerNames.Count > 0)
            {
                enquiryDTO.CustomerNameList = CustomerNames;
                ViewBag.CustomerNameList = enquiryDTO.CustomerNameList;
            }
            if (StatusNames.Count > 0)
            {
                ViewBag.statusType = StatusNames;
            }

            if (RecruitmentNames.Count > 0)
            {
                ViewBag.recruitmentType = RecruitmentNames;
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
                    EnquiryEntity enquiry = new EnquiryEntity();
                    enquiry = MapEntity(enquiry, enquiryEntity);
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
                                return RedirectToAction("dashboard", "staff");
                            }
                            else
                            {
                                return RedirectToAction("dashboard", "staff");
                            }

                        }
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult AddStatusType(string statustypeName)
        {
            AddStatusTypes(statustypeName);
            return RedirectToAction("Enquiry");
        }

        [HttpPost]
        public async Task<ActionResult> AddStatusTypes(string statustypeName)
        {
            StatusType statusType = new StatusType();
            var StatusType = GetStatusByname(statustypeName).Result;
            if (StatusType == null)
            {
                statusType.StatusTypeName = statustypeName;
                await _context.StatusTypes.AddAsync(statusType);
                await _context.SaveChangesAsync();
                return RedirectToAction("Enquiry");
            }
            else
            {
                ViewBag.Error = $"Statustype Name {statustypeName} already exists";
            }
            return RedirectToAction("Enquiry");
        }
        public async Task<StatusType> GetStatusByname(string statustypename)
        {
            StatusType statusType = new StatusType();
            statusType = await _context.StatusTypes.FirstOrDefaultAsync(s => s.StatusTypeName.Equals(statustypename));
            return statusType;
        }
        public async Task<List<String>> GetStatusName()
        {

            List<string> statusType = await _context.StatusTypes.OrderBy(c => c.StatusTypeName)
                .Select(s => s.StatusTypeName).ToListAsync();
            return statusType;
        }

        public async Task<List<String>> GetStatusNameForDashboard()
        {

            List<string> statusType = new List<string>();
            statusType.Add("All");
            statusType = await _context.StatusTypes
                .Where(e => !e.StatusTypeName.Equals("Payment Pending")
                && !e.StatusTypeName.Equals("Rejected") && !e.StatusTypeName.Equals("Completed")
                && !e.StatusTypeName.Equals("New")).OrderBy(e => e.StatusTypeName)
                .Select(s => s.StatusTypeName).ToListAsync();
            return statusType;
        }

        [HttpGet]
        public ActionResult AddRecruitmentType(string recruitmenttypeName)
        {
            AddRecruitmentTypes(recruitmenttypeName);
            return RedirectToAction("Enquiry");
        }

        [HttpPost]
        public async Task<ActionResult> AddRecruitmentTypes(string recruitmenttypeName)
        {
            RecruitmentType recruitmentType = new RecruitmentType();
            var RecruitmentType = GetRecruitmentByname(recruitmenttypeName).Result;
            if (RecruitmentType == null)
            {
                recruitmentType.RecruitmentTypeName = recruitmenttypeName;
                await _context.RecruitmentTypes.AddAsync(recruitmentType);
                await _context.SaveChangesAsync();
                return RedirectToAction("Enquiry");
            }
            else
            {
                ViewBag.Error = $"Recruitment" +
                    $" Name {recruitmenttypeName} already exists";
            }
            return RedirectToAction("Enquiry");
        }

        public async Task<RecruitmentType> GetRecruitmentByname(string recruitmentName)
        {
            RecruitmentType recruitmentType = new RecruitmentType();
            recruitmentType = await _context.RecruitmentTypes.FirstOrDefaultAsync(s => s.RecruitmentTypeName.Equals(recruitmentName));
            return recruitmentType;
        }
        public async Task<List<String>> GetRecruitmentName()
        {

            List<string> recruitmentType = await _context.RecruitmentTypes.OrderBy(s => s.RecruitmentTypeName)
                .Select(s => s.RecruitmentTypeName).ToListAsync();
            return recruitmentType;
        }
        public async Task<List<String>> GetstaffName()
        {

            List<string> staff = await _context.Staffs
                .Select(s => s.StaffName).ToListAsync();
            return staff;
        }
        public EnquiryEntity MapEntity(EnquiryEntity enquiry, EnquiryDTO enquiryDTO)
        {
            enquiry.CustomerName = enquiryDTO.CustomerName;
            enquiry.Status = enquiryDTO.Status;
            enquiry.Recruitment = enquiryDTO.RecruitmentName;
            enquiry.Remark = enquiryDTO.Remark;
            enquiry.Schedule = enquiryDTO.Schedule;
            enquiry.Payment = enquiryDTO.Payment;
            enquiry.Resource = enquiryDTO.Resource;
            return enquiry;
        }

        public EnquiryEntity MapEditEntity(EnquiryEntity enquiry, EditEnquiryDto enquiryDTO)
        {
            enquiry.CustomerName = enquiryDTO.CustomerName;
            enquiry.Status = enquiryDTO.Status;
            enquiry.Recruitment = enquiryDTO.RecruitmentName;
            enquiry.Remark = enquiryDTO.Remark;
            enquiry.Schedule = enquiryDTO.Schedule;
            enquiry.Payment = enquiryDTO.Payment;
            enquiry.Resource = enquiryDTO.Resource;
            return enquiry;
        }

        [HttpGet]
        public async Task<ActionResult> EditEnquiry(int id)
        {
            EditEnquiryDto enquiryDTO = new EditEnquiryDto();
            List<string> CustomerNames = new List<string>();
            List<string> staffnames = new List<string>();
            staffnames = GetstaffName().Result;
            CustomerNames = _customerService.GetCustomersName().Result;
            List<string> StatusNames = new List<string>();
            StatusNames = GetStatusName().Result;
            List<string> RecruitmentNames = new List<string>();
            RecruitmentNames = GetRecruitmentName().Result;

            if (CustomerNames.Count > 0)
            {
                enquiryDTO.CustomerNameList = CustomerNames;
                ViewBag.CustomerNameList = enquiryDTO.CustomerNameList;
            }

            if (StatusNames.Count > 0)
            {
                ViewBag.statusType = StatusNames;
            }

            if (RecruitmentNames.Count > 0)
            {
                ViewBag.recruitmentType = RecruitmentNames;
            }
            if (staffnames.Count > 0)
            {
                ViewBag.resource = staffnames;
            }

            var enquiry = GetEnquiryById(id).Result;
            enquiryDTO.CustomerName = enquiry.CustomerName;
            enquiryDTO.Customer = enquiry.Customer;
            enquiryDTO.RecruitmentName = enquiry.Recruitment;
            enquiryDTO.Remark = enquiry.Remark;
            enquiryDTO.Status = enquiry.Status;
            enquiryDTO.Schedule = enquiry.Schedule;
            enquiryDTO.Payment = enquiry.Payment;
            enquiryDTO.Resource = enquiry.Resource;
            return View(enquiryDTO);
        }

        [HttpPost]
        public async Task<ActionResult> EditEnquiry(EditEnquiryDto enquiryDTO)
        {
            var enquiry = GetEnquiryById(enquiryDTO.Id).Result;
            enquiryDTO.CustomerName = enquiry.CustomerName;
            enquiryDTO.RecruitmentName = enquiry.Recruitment;

            if (enquiryDTO != null && ModelState.IsValid)
            {
                enquiry = MapEditEntity(enquiry, enquiryDTO);
                Customer customer = _customerService.GetCustomerByName(enquiryDTO.CustomerName).Result;
                enquiry.CustomerId = customer.CustomerId;
                enquiry.UpdatedOn = DateTime.Now;
                enquiry.UpdatedBy = HttpContext.Session.GetString("LoggedInUserName");
                _context.Enquiries.Update(enquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction("adminDashboard");
            }
            else
            {
                ViewBag.error = "Please enter required fields";
                return View();
            }
        }
        [HttpGet]
        public async Task<ActionResult> DeleteEnquiry(int id)
        {
            EnquiryEntity enquiry = GetEnquiryById(id).Result;
            enquiry.IsDeleted = true;
            enquiry.UpdatedOn = DateTime.Now;
            enquiry.UpdatedBy = HttpContext.Session.GetString("LoggedInUserName");
            _context.Enquiries.Update(enquiry);
            await _context.SaveChangesAsync();
            return RedirectToAction("dashboard");
        }
        public async Task<EnquiryEntity> GetEnquiryById(int enquiryId)
        {
            var enquiry = await _context.Enquiries.Include(c => c.Customer)
                .FirstOrDefaultAsync(e => e.Id == enquiryId);
            return enquiry;
        }
        [HttpGet]
        public async Task<ActionResult> EditStaff(int id)
        {
            Staff staff = new Staff();
            var staffid = GetstaffById(id).Result;
            staff.StaffName = staffid.StaffName;
            staff.Password = staffid.Password;
            staff.ConfirmPassword = staffid.ConfirmPassword;
            staff.StaffType=staffid.StaffType;
            staff.StaffId = staffid.StaffId;
            return View(staff);
        }
        [HttpPost]
        public async Task<ActionResult> EditStaff(Staff staff)
        {
            var staff1 = GetstaffById(staff.StaffId).Result;
            if (staff != null && ModelState.IsValid)
            {
                staff1.StaffName = staff.StaffName;
                staff1.Password = staff.Password; 
                staff1.ConfirmPassword = staff.ConfirmPassword;
                _context.Staffs.Update(staff1);
                await _context.SaveChangesAsync();
                return RedirectToAction("displaystaff");
            }
            else
            {
                ViewBag.error = "Please enter required fields";
                return View();
            }
        }
    }
}