using System;
using System.Linq;

namespace Order.Data.Specifications
{
    public class OrderByIdSpecification : BaseEmptySpecification<Entities.Order>
    {
        public OrderByIdSpecification(Guid orderId, bool isInMemoryDatabase)
        {
            var orderIdBytes = orderId.ToByteArray();
            Query = x => isInMemoryDatabase ? x.Id.SequenceEqual(orderIdBytes) : x.Id == orderIdBytes;
        }
    }
}
