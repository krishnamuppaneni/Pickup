using Pickup.Entity.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pickup.Entity
{
    public class Delivery : BaseEntity
    {
        [Key]
        public int DeliveryId { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }

        public int DeliveryLocationId { get; set; }

        public int DriverLocationId { get; set; }

        public int PickupListId { get; set; }

        public string DriverUserId { get; set; }

        public string DeliveryUserId { get; set; }

        public int ReviewId { get; set; }

        [ForeignKey("DeliveryLocationId")]
        public virtual Location DeliveryLocation { get; set; }

        [ForeignKey("DriverLocationId")]
        public virtual Location DriverLocation { get; set; }

        [ForeignKey("PickupListId")]
        public virtual PickupList PickupList { get; set; }

        [ForeignKey("DriverUserId")]
        public virtual User DriverUser { get; set; }

        [ForeignKey("DeliveryUserId")]
        public virtual User DeliveryUser { get; set; }

        [ForeignKey("ReviewId")]
        public virtual Review Review { get; set; }
    }

    public enum DeliveryStatus
    {
        Cancelled,
        Delivered,
        Inprogress,
        Reported
    }
}
