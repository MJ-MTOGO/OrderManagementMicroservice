using Microsoft.EntityFrameworkCore;
using OrderManagementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace OrderManagementService.Infrastructure
{
    public class OrderManagementServiceDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public OrderManagementServiceDbContext(DbContextOptions<OrderManagementServiceDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId); // Primary Key

                entity.Property(o => o.CustomerId).IsRequired(); // CustomerId is required
                entity.Property(o => o.RestaurantId).IsRequired(); // RestaurantId is required
                entity.Property(o => o.OrderedTime).IsRequired(); // OrderedTime is required
                entity.Property(o => o.OrderStatus).IsRequired(); // OrderStatus is required

                entity.OwnsMany(o => o.OrderItems, item =>
                {
                    item.WithOwner(); // Establish ownership (Order owns OrderItem)
                    item.Property(i => i.Name).IsRequired(); // Name is required
                    item.Property(i => i.Price).IsRequired()
                    .HasPrecision(18, 2);// Price is required
                });
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
