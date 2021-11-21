namespace Order.Data.Specifications.OrderSummarySpec
{
    public class OrderSummarysByStatusOrderedByDateSpecification : OrderSummarySpecification
    {
        public OrderSummarysByStatusOrderedByDateSpecification(string status): base()
        {
            Query = o => o.Status.Name.Equals(status);
        }
    }
}
