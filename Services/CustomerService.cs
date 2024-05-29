using Microsoft.EntityFrameworkCore;
using TallySoftware.Entity;

namespace TallySoftware.Services
{
    public class CustomerService:ICustomerService
    {

            private readonly ApplicationDbContext _context;
            public CustomerService(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<string>> GetCustomersName()
        {
          List<String> CustomerNames= _context.Customers.Where(c => !c.IsDeleted).Select(x => x.Name).ToList();
           
            return CustomerNames;
        }
        public async Task<List<Customer>> GetCustomers()
        {

            List<Customer> customers = new List<Customer>();
            customers = _context.Customers.Include(c => c.enquiries).Where(c => !c.IsDeleted).ToList();
            return customers;
        }

        public async Task<Customer> GetCustomerByName(string name)
        {
            Customer customer = new Customer();
            customer = await _context.Customers.FirstOrDefaultAsync(c => !c.IsDeleted &&  c.Name.Equals(name));
            return customer;
        }
        public async Task<List<StatusType>> GetStatusTypes()
        {
            List<StatusType>statustypes= new List<StatusType>();
            statustypes=await _context.StatusTypes.ToListAsync();
            return statustypes;
        }
        public async Task<List<RecruitmentType>> GetRecruitmentTypes()
        {
            List<RecruitmentType>recruitmentTypes= new List<RecruitmentType>();
            recruitmentTypes=await _context.RecruitmentTypes.ToListAsync();
            return recruitmentTypes;
        }
    }
}
