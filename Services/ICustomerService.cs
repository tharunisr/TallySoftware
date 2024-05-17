using TallySoftware.Entity;

namespace TallySoftware.Services
{
    public interface ICustomerService
    {
        Task<List<string>> GetCustomersName();
        Task<List<Customer>> GetCustomers();
        Task<Customer> GetCustomerByName(string name);
    }
}
