using System;
using System.Linq;

namespace Order.Data.Specifications.OrderSummarySpec
{
    public class OrderSummaryByIdSpecification : OrderSummarySpecification
    {
        public OrderSummaryByIdSpecification(Guid orderId, bool isInMemoryDatabase) : base()
        {
            var orderIdBytes = orderId.ToByteArray();
            Query = x => isInMemoryDatabase ? x.Id.SequenceEqual(orderIdBytes) : x.Id == orderIdBytes;
        }
    }
}
