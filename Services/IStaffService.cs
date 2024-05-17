using TallySoftware.Entity;

namespace TallySoftware.Services
{
    public interface IStaffService
    {
        Task<Staff> GetStaff(string username);
        Task<Staff> GetStaff(string username, string password);
    }
}
