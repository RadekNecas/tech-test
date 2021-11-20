using Order.Service.Specifications;

namespace Order.WebAPI.ViewModels
{
    public class GetOrdersParameters
    {
        public string Status { get; set; }

        public OrderSpecification AsOrderSpecification() => new OrderSpecification(Status);
    }
}
