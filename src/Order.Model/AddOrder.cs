using Order.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Order.Model
{
    public class AddOrder
    {
        [Required]
        public Guid ResellerId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }


        [Required]
        public IEnumerable<AddOrderItem> Items { get; set; }
    }
}
