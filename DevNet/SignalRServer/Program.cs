using Microsoft.AspNetCore.SignalR;
using SignalRServer.SignalR;

var builder = WebApplication.CreateBuilder(args);
// Add SignalR and app services
builder.Services.AddSingleton<IUserIdProvider, QueryStringUserIdProvider>();
builder.Services.AddSignalR();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapHub<ChatHub>("/hub");

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();
