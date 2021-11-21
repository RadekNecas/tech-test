using Order.Data.Entities;
using Order.Data.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Data
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<OrderProduct>> GetProducts(ISpecification<OrderProduct> specification);
    }
}