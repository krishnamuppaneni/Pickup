using System;
using System.ComponentModel.DataAnnotations;

namespace Pickup.Data.Entities.Common
{
    public abstract class BaseEntity
    {
        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
