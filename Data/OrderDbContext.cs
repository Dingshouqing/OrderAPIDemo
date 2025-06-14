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
                .IsRequired();

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
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();

            // Seed Orders
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    OrderId = orderId1,
                    CustomerName = "John Doe",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Order
                {
                    OrderId = orderId2,
                    CustomerName = "Jane Smith",
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                }
            );

            // Seed OrderItems
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem
                {
                    Id = 1,
                    OrderId = orderId1,
                    ProductId = "PROD001",
                    Quantity = 2
                },
                new OrderItem
                {
                    Id = 2,
                    OrderId = orderId1,
                    ProductId = "PROD002",
                    Quantity = 1
                },
                new OrderItem
                {
                    Id = 3,
                    OrderId = orderId2,
                    ProductId = "PROD003",
                    Quantity = 3
                }
            );
        }
    }
}
