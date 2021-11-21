using Order.Service.Specifications;

namespace Order.WebAPI.ViewModels
{
    public class GetOrdersParameters
    {
        public string Status { get; set; }

        public ListOrdersSpecification AsOrderSpecification() => new ListOrdersSpecification(Status);
    }
}
