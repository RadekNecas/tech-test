namespace Order.Service.Specifications
{
    public class OrderSpecification
    {
        public static OrderSpecification AllOrders = new OrderSpecification();

        public OrderSpecification()
        {
        }

        public OrderSpecification(string status)
        {
            Status = status;
        }

        public string Status { get; set; }
    }
}
