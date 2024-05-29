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
        private string? status = null;
        private string? recruitment = null;
        private string? search = null;
        public StaffController(ApplicationDbContext context, IStaffService staffService, ICustomerService customerService)
        {
            _context = context;
            _staffService = staffService;
            _customerService = customerService;
        }
        [HttpGet]
        public async Task<ActionResult> AdminDashboard(DateTime? schedule = null, string status = null,
            string recruitment = null, string search = null,int pageno=1)
        {
            this.status = status;
            this.recruitment = recruitment;
            this.search = search;
            ViewBag.schedule = schedule !=null ? DateOnly.FromDateTime(schedule.Value).ToString("yyyy-MM-dd")
                : null;
            List <EnquiryEntity> enquiries = new List<EnquiryEntity>();
            var query = _context.Enquiries.Include(c => c.Customer)
               .Where(e => !e.IsDeleted && !e.Customer.IsDeleted).AsQueryable();
            List<string> RecruitmentNames = new List<string>();
            RecruitmentNames = GetRecruitmentName().Result;

            List<string> StatusNames = new List<string>();
            StatusNames = GetStatusName().Result;

            if (StatusNames.Count > 0)
            {
                ViewBag.statusType = StatusNames;
            }
            if (RecruitmentNames.Count > 0)
            {
                ViewBag.recruitmentType = RecruitmentNames;
            }
            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(c => c.Status.Equals(this.status));
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
                query = query.Where(c => c.CustomerName.Equals(search) 
                || c.Customer.PhoneNumber.Equals(search));
            }

            enquiries = query.ToList();
            ViewBag.TotalCount = enquiries.Count;
            int noofrecordperpage = 10;
            int noofpage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(enquiries.Count) / Convert.ToDouble(noofrecordperpage)));
            int noofrecordstoskip = (pageno - 1) * noofrecordperpage;
            ViewBag.pageno = pageno;
            ViewBag.noofpage = noofpage;
            enquiries = enquiries.Skip(noofrecordstoskip).Take(noofrecordperpage).ToList();
            return View(enquiries);
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
            enquiryDTO.Schedule=DateTime.Now;
            List<string> CustomerNames = new List<string>();
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

            List<string> statusType = await _context.StatusTypes.Select(s => s.StatusTypeName).ToListAsync();
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

            List<string> recruitmentType = await _context.RecruitmentTypes.Select(s => s.RecruitmentTypeName).ToListAsync();
            return recruitmentType;
        }
        public EnquiryEntity MapEntity(EnquiryEntity enquiry, EnquiryDTO enquiryDTO)
        {
            enquiry.CustomerName = enquiryDTO.CustomerName;
            enquiry.Status = enquiryDTO.Status;
            enquiry.Recruitment = enquiryDTO.RecruitmentName;
            enquiry.Remark = enquiryDTO.Remark;
            enquiry.Schedule = enquiryDTO.Schedule;
            enquiry.Payment = enquiryDTO.Payment;
            return enquiry;
        }

        [HttpGet]
        public async Task<ActionResult> EditEnquiry(int id)
        {
            EnquiryDTO enquiryDTO = new EnquiryDTO();
            List<string> CustomerNames = new List<string>();
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
            var enquiry = GetEnquiryById(id).Result;
            enquiryDTO.CustomerName = enquiry.CustomerName;
            enquiryDTO.Customer = enquiry.Customer ;
            enquiryDTO.RecruitmentName = enquiry.Recruitment;
            enquiryDTO.Remark = enquiry.Remark;
            enquiryDTO.Status = enquiry.Status;
            enquiryDTO.Schedule=enquiry.Schedule;
            enquiryDTO.Payment=enquiry.Payment;
            return View(enquiryDTO);
        }
        [HttpPost]
        public async Task<ActionResult> EditEnquiry(EnquiryDTO enquiryDTO)
        {
            if (enquiryDTO != null && ModelState.IsValid)
            {
                var enquiry = GetEnquiryById(enquiryDTO.Id).Result;
                enquiry = MapEntity(enquiry, enquiryDTO);
                Customer customer = _customerService.GetCustomerByName(enquiryDTO.CustomerName).Result;
                enquiry.CustomerId = customer.CustomerId;
                enquiry.UpdatedOn = DateTime.Now;
                enquiry.UpdatedBy = HttpContext.Session.GetString("LoggedInUserName");
                _context.Enquiries.Update(enquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminDashboard");
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
            return RedirectToAction("Admindashboard");
        }
        public async Task<EnquiryEntity> GetEnquiryById(int enquiryId)
        {
            var enquiry = await _context.Enquiries.Include(c => c.Customer)
                .FirstOrDefaultAsync(e => e.Id == enquiryId);
            return enquiry;
        }
    }
}