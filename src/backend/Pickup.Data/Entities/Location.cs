using NetTopologySuite.Geometries;
using Pickup.Data.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pickup.Data.Entities
{
    public class Location : BaseEntity
    {
        [Key]
        public int LocationId { get; set; }

        public string UserId { get; set; }

        public Point UserLocation { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
