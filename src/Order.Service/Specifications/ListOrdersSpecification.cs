namespace Order.Service.Specifications
{
    public class ListOrdersSpecification
    {
        public ListOrdersSpecification()
        {
        }

        public ListOrdersSpecification(string status)
        {
            Status = status;
        }

        public string Status { get; set; }
    }
}
