using Order.Model;

namespace Order.Data.Dto
{
    public class OrderSummaryDto : OrderSummary
    {
        public int SystemId { get; set; }
        public int SystemResellerId { get; set; }
        public int SystemCustomerId { get; set; }
        public int SystemStatusId { get; set; }
    }
}
