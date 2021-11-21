using Order.Model.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Order.Data.Entities
{
    public class AddOrderItem
    {
        [RequiredNotEmpty]
        public string ProductName { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
