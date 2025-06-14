
using Microsoft.EntityFrameworkCore;
using OrderSvc.DataContract;
using OrderSvc.WebAPI.Data;
using OrderSvc.WebAPI.Repositories;
using OrderSvc.WebAPI.Repositories.Concrete;
using OrderSvc.WebAPI.Services;
using OrderSvc.WebAPI.Services.Concrete;
using Serilog;

namespace OrderWebAPI
{
    public class Program
    {
        private const string LogPath = "logs\\log.txt";

        /// <summary>
        /// The main entry point for the Order Web API application
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(LogPath, rollingInterval: RollingInterval.Day)
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
                if (!builder.Environment.EnvironmentName.Equals(Constrants.TestEnvironment))
                    options.UseSqlite(builder.Configuration.GetConnectionString(Constrants.DBConnectionStringKey));
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
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<OrderDbContext>();
                context.Database.EnsureCreated();
            }

            app.Run();
        }
    }
}
