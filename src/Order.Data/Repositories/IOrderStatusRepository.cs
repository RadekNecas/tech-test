using Order.Data.Entities;
using Order.Data.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Order.Data
{
    public interface IOrderStatusRepository
    {
        Task<OrderStatus> GetOrderStatusAsync(ISpecification<OrderStatus> specification);
    }
}
