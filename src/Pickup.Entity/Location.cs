using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using Pickup.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pickup.Entity
{
    public class Location : BaseEntity
    {
        public int LocationId { get; set; }

        public string UserId { get; set; }

        public Point UserLocation { get; set; }

        public LocationType LocationType { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

    public enum LocationType
    {
        Current,
        Delivery,
        Review,
        Report
    }
}
