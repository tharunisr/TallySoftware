using System.ComponentModel.DataAnnotations;
namespace TallySoftware.Entity
{
    public class Staff
    {
        public int StaffId { get; set; }
        [Required]
        [Display(Name="Username")]
        public string StaffName {  get; set; }
        [Required]
        public string Password { get; set; }
        [Required]

        [Compare("Password", ErrorMessage = "Should be same as Password ")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        public string? StaffType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
