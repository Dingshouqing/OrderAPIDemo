
using Microsoft.EntityFrameworkCore;
using OrderWebAPI.Data;
using OrderWebAPI.Repositories;
using OrderWebAPI.Repositories.Concrete;
using OrderWebAPI.Services;
using OrderWebAPI.Services.Concrete;
using Serilog;

namespace OrderWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            builder.Host.UseSerilog();

            builder.Services.AddControllers()
                // Ignore json objec loops to prevent stack overflow, and ignore null values
                .AddJsonOptions(options =>
                {
                    // Ignore null values in JSON serialization
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                    // Ignore reference loops
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            // Configure Entity Framework Core
            builder.Services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("OrderContext"));
            });

            // Register repositories
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "OrderWebAPI", Version = "v1", Description = "Order service API" });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderWebAPI v1");
                    c.RoutePrefix = string.Empty; // Serve the Swagger UI at the app's root
                });
            }

            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            // Ensure database is created and seeded
            using(var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<OrderDbContext>();
                context.Database.EnsureCreated();
            }

            app.Run();
        }
    }
}
