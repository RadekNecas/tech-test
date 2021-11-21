using Order.Data.Entities;

namespace Order.Data.Specifications
{
    public class OrderStatusByNameSpecification : BaseEmptySpecification<OrderStatus>
    {
        public OrderStatusByNameSpecification(string statusName)
        {
            Query = x => x.Name.Equals(statusName);
        }
    }
}
