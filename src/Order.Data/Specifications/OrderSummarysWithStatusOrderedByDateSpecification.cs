namespace Order.Data.Specifications
{
    public class OrderSummarysWithStatusOrderedByDateSpecification : OrderSummarySpecification
    {
        public OrderSummarysWithStatusOrderedByDateSpecification(string status): base()
        {
            Query = o => o.Status.Name.Equals(status);
        }
    }
}
