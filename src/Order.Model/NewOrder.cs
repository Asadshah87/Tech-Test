using Order.Model.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Order.Model
{
    public class NewOrder
    {
        [RequiredGuid]
        public Guid? ResellerId { get; set; }
        [RequiredGuid]
        public Guid? CustomerId { get; set; }
        public IEnumerable<NewOrderItem> Items { get; set; }
    }
}
