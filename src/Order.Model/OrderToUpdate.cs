using System.ComponentModel.DataAnnotations;

namespace Order.Model
{
    public class OrderToUpdate
    {
        [Required]
        public string StatusName { get; set; }
    }
}
