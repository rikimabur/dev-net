using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using multiple_authentication_schemes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtPublicOptions>(
    builder.Configuration.GetSection(JwtPublicOptions.SectionName));

builder.Services.Configure<JwtInternalOptions>(
    builder.Configuration.GetSection(JwtInternalOptions.SectionName));

builder.Services.AddAuthentication()
    .AddCookie(AuthSchemes.Cookie, options =>
    {
        options.LoginPath = "/auth/login";
        options.AccessDeniedPath = "/auth/forbidden";
    })
    .AddJwtBearer(AuthSchemes.JwtPublic)
    .AddJwtBearer(AuthSchemes.JwtInternal);
// --------------------
// JWT Setup via Options
// --------------------
builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(
    sp => new JwtBearerOptionsSetup<JwtPublicOptions>(
        sp.GetRequiredService<IOptions<JwtPublicOptions>>(),
        AuthSchemes.JwtPublic));

builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(
    sp => new JwtBearerOptionsSetup<JwtInternalOptions>(
        sp.GetRequiredService<IOptions<JwtInternalOptions>>(),
        AuthSchemes.JwtInternal));

// Add services to the container.

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
