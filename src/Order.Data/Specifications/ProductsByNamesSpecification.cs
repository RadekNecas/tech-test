using Order.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Order.Data.Specifications
{
    public class ProductsByNamesSpecification : BaseEmptySpecification<OrderProduct>
    {
        public ProductsByNamesSpecification(IEnumerable<string> productNames)
        {
            Query = p => productNames.Contains(p.Name);
            OrderBy = p => p.Name;
        }
    }
}
