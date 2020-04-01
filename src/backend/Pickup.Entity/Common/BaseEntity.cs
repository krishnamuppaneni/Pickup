using System;
using System.ComponentModel.DataAnnotations;

namespace Pickup.Entity.Common
{
    public abstract class BaseEntity
    {
        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
