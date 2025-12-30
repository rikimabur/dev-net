using EshopApplication.Abstractions;
using EshopApplication.Orders;
using EshopApplication.Orders.Events;
using EshopDomain.Events;
using EshopDomain.Repositories;
using EshopInfrastructure.Dispatching;
using EshopInfrastructure.DomainEvents;
using EshopInfrastructure.Email;
using EshopInfrastructure.Persistence;
using EshopInfrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddOpenApi()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();



public static class DependencyInjection
{
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblies(
                typeof(DependencyInjection).Assembly,
                typeof(OrderCreatedEvent).Assembly,// Domain
                typeof(PublishOrderCreatedIntegrationEventHandler).Assembly,// Application
                typeof(SendOrderConfirmationEmailHandler).Assembly);// Infrastructure
        });
        // Including Mediator, validation behaviors, and registering application services
        return services;
    }
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
        opts.UseSqlServer(configuration.GetConnectionString("ConnectionString")));
        services.AddScoped<IEmailSender, SmtpEmailSender>();


        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
public partial class Program { }