using Microsoft.EntityFrameworkCore;
using Pickup.Data.Entities;
using Pickup.Data.Entities.Common;
using System;
using System.Linq;

namespace Pickup.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<PickupList> PickupLists { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).ModifiedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}
