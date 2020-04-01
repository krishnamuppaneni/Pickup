using Pickup.Entity.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pickup.Entity
{
    public class PickupList : BaseEntity
    {
        [Key]
        public int PickupListId { get; set; }

        public string ListName { get; set; }

        public string DescriptionText { get; set; }

        public PickupListStatus PickupListStatus { get; set; }

        public int DeliveryLocationId { get; set; }

        [ForeignKey("DeliveryLocationId")]
        public virtual Location DeliveryLocation { get; set; }
    }

    public enum PickupListStatus
    {
        Active,
        Deleted,
        Delivered,
        Inprogress, 
        Reported
    }
}
