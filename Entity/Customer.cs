using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace TallySoftware.Entity
{
    public class Customer
    {
        public int CustomerId { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Address { get; set; }
        [Required]
        [RegularExpression("/^(\\+)(?(1))\\d{1,3}?[- ]?\\d{10}$|^([^0])(?(1))\\d{9}?$|^([0])(?(1))\\d{10}?$/")]
        public string PhoneNumber { get; set; }
        public string? Remark { get; set; }
        
        [Display(Name = "AdministratorId")]
        public int? AdministrativeId {  get; set; }
        public string? CompanyName {  get; set; }
        public string? ContactPersonName { get; set; }
        public int? CustomerTypeId { get; set; }
        public string? CustomerTypeName { get; set; }
        public CustomerType CustomerType { get; set; }
        public string? CreatedBy {  get; set; }
        public DateTime? CreatedOn { get; set;}
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set;}
        public bool IsDeleted { get; set; }
        public ICollection<EnquiryEntity> enquiries {  get; set; }
    }
}
