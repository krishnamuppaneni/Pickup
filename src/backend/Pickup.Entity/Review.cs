using Pickup.Entity.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pickup.Entity
{
    public class Review : BaseEntity
    {
        [Key]
        public int ReviewId{ get; set; }

        public string DriverUserId { get; set; }

        public string DeliveryUserId { get; set; }

        public int DeliveryId { get; set; }

        public string CommentText { get; set; }

        public float RatingNumb { get; set; }

        public ReviewStatus ReviewStatus { get; set; }

        [ForeignKey("DriverUserId")]
        public virtual User DriverUser { get; set; }

        [ForeignKey("DeliveryUserId")]
        public virtual User DeliveryUser { get; set; }

        [ForeignKey("DeliveryId")]
        public virtual Delivery Delivery { get; set; }

    }

    public enum ReviewStatus
    {
        Active,
        Deleted,
        Inactive,
        Updated
    }
}
