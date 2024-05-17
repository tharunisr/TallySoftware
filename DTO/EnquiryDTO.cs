using System.ComponentModel.DataAnnotations;

namespace TallySoftware.DTO
{
    public class EnquiryDTO
    {
        [Required]
        public string CustomerName {  get; set; }
        public List<string>? CustomerNameList { get; set; }
        public List<string>? RecruitmentList { get; set; }
        [Required]
        public string RecruitmentName { get; set; }
        [Required]
        public string Status {  get; set; }
        [Required]
        public string Remark {  get; set; }
        [Required]
        public DateTime Schedule {  get; set; }
        [Required]
        public string Payment {  get; set; }

    }
}
