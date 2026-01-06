using OrderApi.Behaviors;
using OrderApi.Commands;
using OrderApi.Mediator;
using OrderApi.Queries;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();

// Add services to the container.

// Manually
//builder.Services.AddSingleton<IMediator, Mediator>();
//builder.Services.AddTransient<IRequestHandler<CreateOrderCommand, CreateOrderResponse>, CreateOrderHandler>();
//builder.Services.AddTransient<IRequestHandler<GetOrderQuery, GetOrderResponse>, GetOrderHandler>();
// pipeline behaviors
//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MetricsBehavior<,>));
//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));


// Automatically 
builder.Services.AddMediator(typeof(Program).Assembly);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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
