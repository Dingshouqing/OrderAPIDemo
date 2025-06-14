using Microsoft.EntityFrameworkCore;
using OrderWebAPI.Model;

namespace OrderWebAPI.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // configure order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
               
                entity.Property(p => p.CustomerName)
                .IsRequired()
                .HasMaxLength(100);
                
                entity.Property(p => p.CreatedAt)
                .IsRequired(false);

                entity.HasMany(e => e.OrderItems)
                .WithOne(e => e.Order)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            // configure order item entity
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(p => p.ProductId)
                .IsRequired()
                .HasMaxLength(50);

                entity.Property(p => p.OrderId)
                .IsRequired();

                entity.Property(p => p.Quantity)
                .IsRequired();
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // seed data
            var order1 = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Apple",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = "P1",
                        Quantity = 1
                    },
                    new OrderItem
                    {
                        ProductId = "P2",
                        Quantity = 2
                    }
                }
            };
            modelBuilder.Entity<Order>().HasData(order1);

            var order2 = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Dell",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = "P3",
                        Quantity = 3
                    },
                    new OrderItem
                    {
                        ProductId = "P4",
                        Quantity = 4
                    }
                }
            };
            modelBuilder.Entity<Order>().HasData(order2);
        }
    }
}
