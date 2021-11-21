namespace Order.Data.Specifications.OrderSummarySpec
{
    public class OrderSummarysWithStatusOrderedByDateSpecification : OrderSummarySpecification
    {
        public OrderSummarysWithStatusOrderedByDateSpecification(string status): base()
        {
            Query = o => o.Status.Name.Equals(status);
        }
    }
}
