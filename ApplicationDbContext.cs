using Microsoft.EntityFrameworkCore;
using TallySoftware.Entity;
namespace TallySoftware
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            AdminSeeding.Seed(modelBuilder);
        }



        public DbSet<Staff>Staffs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<EnquiryEntity> Enquiries { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<StatusType> StatusTypes { get; set; }
        public DbSet<RecruitmentType> RecruitmentTypes { get; set;}
    }
}
