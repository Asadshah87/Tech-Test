using Order.Model.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Order.WebAPI.Dtos
{
    public class UpdateOrderStatus
    {
        [RequiredGuid]
        public Guid? Id { get; set; }

        [RequiredGuid]
        public Guid? OrderStatusId { get; set; }
    }
}
