namespace TallySoftware.Entity
{
    public class CustomerType
    {
        public int CustomerTypeId {  get; set; }
        public string CustomerTypeName { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
