using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Data;
using Order.Data.Repositories;
using Order.Data.Specifications.Evaluators;
using Order.Service;
using Order.WebAPI.Middlewares;

namespace OrderService.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OrderContext>(options =>
            {
                var serviceOptions = Configuration["OrderConnectionString"];
                options
                .UseLazyLoadingProxies()
                .UseMySQL(serviceOptions);
            });

            services.AddScoped<IOrderService, Order.Service.OrderService>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<ISpecificationEvaluator, SpecificationEvaluator>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
