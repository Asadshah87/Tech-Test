using Order.Model.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Order.Model
{
    public class NewOrderItem
    {
        [RequiredGuid]
        public Guid? ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid quantity")]
        public int? Quantity { get; set; }
    }
}
