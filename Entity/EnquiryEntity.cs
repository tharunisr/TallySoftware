using System.ComponentModel.DataAnnotations;

namespace TallySoftware.Entity
{
    public class EnquiryEntity
    {
        public int Id {  get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string Recruitment { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Remark {  get; set; }
        [Required]
        public DateTime Schedule {  get; set; }
        [Required]
        public string Payment {  get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy {  get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public Customer Customer { get; set; }

    }
}
