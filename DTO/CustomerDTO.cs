using System.ComponentModel.DataAnnotations;

namespace TallySoftware.DTO
{
    public class CustomerDTO
    {
        public int Customerid { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Address { get; set; }
        [Required]
        [RegularExpression("/^(\\+)(?(1))\\d{1,3}?[- ]?\\d{10}$|^([^0])(?(1))\\d{9}?$|^([0])(?(1))\\d{10}?$/")]
        public string PhoneNumber { get; set; }
        public string? Remark { get; set; }
        public int? AdministrativeId { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactPersonName { get; set; }
        public int? CustomerTypeId { get; set; }
        public string? CustomerTypeName { get; set; }
    }
}
