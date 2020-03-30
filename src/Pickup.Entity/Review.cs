using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Pickup.Entity.Common;

namespace Pickup.Entity
{
    public class Review : BaseEntity
    {
        [Key]
        public int ReviewId{ get; set; }

        public string FromUserID { get; set; }

        public string ToUserID { get; set; }

        public string CommentText { get; set; }

        public float RatingNumb { get; set; }

        public ReviewStatus ReviewStatus { get; set; }

        public int ReviewLocationId { get; set; }

        [ForeignKey("FromUserID")]
        public virtual User FromUser { get; set; }

        [ForeignKey("ToUserID")]
        public virtual User ToUser { get; set; }

        [ForeignKey("ReviewLocationId")]
        public virtual Location ReviewLocation { get; set; }
    }

    public enum ReviewStatus
    {
        Active,
        Deleted,
        Inactive,
        Updated
    }
}
