﻿using TallySoftware.Entity;

namespace TallySoftware.Services
{
    public interface ICustomerService
    {
        Task<List<string>> GetCustomersName();
        Task<List<Customer>> GetCustomers();
        Task<Customer> GetCustomerByName(string name);
        Task<List<StatusType>> GetStatusTypes();
        Task<List<RecruitmentType>>GetRecruitmentTypes ();
    }
}
