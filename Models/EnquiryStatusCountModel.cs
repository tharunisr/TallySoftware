namespace TallySoftware.Models
{
    public class EnquiryStatusCountModel
    {
        public int Lead { get; set; }
        public int PaymentPending { get; set; }
        public int Completed { get; set; }
        public int Rejected { get; set; }
        public int New { get; set; }
        public int TodaySchedule { get; set; }
    }
}
