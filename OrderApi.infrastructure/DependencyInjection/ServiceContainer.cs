using eCommmerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // Add Database Connectivity
            // Add authentication scheme
            // Example: services.AddAuthentication(...);

            // Add shared services
            SharedServiceContainer.AddSharedServices<OrderDbContext>(services, config, config["MySerilog.FileName"]!);

            // Create Dependency Injection
            services.AddScoped<IOrder, OrderRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middleware such as:
            // Global Exception -> handle external errors
            // Example of global exception handling

            // ListenToApiGateway Only -> block all outsiders calls
            // Example: app.UseMiddleware<ApiGatewayMiddleware>();

            // Use shared policies
            SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}