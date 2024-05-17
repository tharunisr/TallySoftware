using Microsoft.EntityFrameworkCore;
using TallySoftware.Entity;

namespace TallySoftware.Services
{
    public class StaffService:IStaffService
    {
        private readonly ApplicationDbContext _context;
        public StaffService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Staff> GetStaff(string username )   
            {
            Staff? staff = await _context.Staffs.
                FirstOrDefaultAsync(u => u.StaffName.Equals(username));
            if (staff == null)
            {
                return null;
            }
            else
            {
                return staff;
            }
        }
        public async Task<Staff> GetStaff(string username, string password)
        {
            Staff? staff = await _context.Staffs.
                FirstOrDefaultAsync(u => u.StaffName.Equals(username) && u.Password.Equals(password));
            if (staff == null)
            {
                return null;
            }
            else
            {
                return staff;
            }
        }

    }
}
