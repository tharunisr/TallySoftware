using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallySoftware.Entity;
using System.Web;
using TallySoftware.Services;
using Azure;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using System;

namespace TallySoftware.Controllers
{
    
    public class AuthenticationController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly ApplicationDbContext _context;
       public AuthenticationController(ApplicationDbContext context, IStaffService staffService) { 
            _context = context;
            _staffService = staffService;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(Staff staff) { 
            try
            {
                if (!string.IsNullOrWhiteSpace(staff.StaffName) && !string.IsNullOrWhiteSpace(staff.Password) )
                {
                    Staff user = _staffService.GetStaff(staff.StaffName, staff.Password).Result;
                    if (user == null)
                    {
                        ViewBag.Error = "Staff not found";
                    }
                    else
                    {
                        HttpContext.Session.SetString("LoggedInUserName", user.StaffName[0]
                            .ToString().ToUpper() + user.StaffName.Substring(1,user.StaffName.Length-1));
                        HttpContext.Session.SetString("LoggedInUserType",user.StaffType);
                        
                        
                        if (user.StaffType == "Admin")
                        {
                            return RedirectToAction("dashboard", "Staff");
                        }
                        else
                        {
                            return RedirectToAction("dashboard", "Staff");
                        }
                       
                    }
                }
                else
                {
                    ViewBag.Error = "Staff not found";
                }
            }
            catch(Exception ex)
            {
                ViewBag.Error=ex.Message;
            } 
         
            return View();
        }
        [HttpGet]
        public ActionResult Logout()
        {
            //Session["loggedInUser"] = null;


            HttpContext.Session.Clear();
            return View("login");
        }

        //protected Staff GetuserByName(string username)
        //{
        //    if (!string.IsNullOrWhiteSpace(username))
        //    {
        //        return context.Staffs.FirstOrDefault(u => u.StaffName.Equals(username));
        //    }
        //    return null;
        //}
        //protected bool CheckPassword(UserEntity user, string password)
        //{
        //    return password.Equals(user.Password);
        //}


    }
}
