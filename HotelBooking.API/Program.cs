using HotelBooking.Data.Services;
using HotelBooking.Domain.Services;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(x 
                    => { x.SuppressMapClientErrors = true; });

builder.Services.AddDataConfig();
builder.Services.AddDomainConfig();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Booking API",
        Description = "An ASP.NET Core Web API for managing Bookings to a single hotel room",
        Contact = new OpenApiContact
        {
            Name = "Victor dos Santos",
            Url = new Uri("https://github.com/m1thr4nd1r")
        },
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    var domainAssembly = Assembly.GetAssembly(typeof(HotelBooking.Domain.Services.DomainConfigExtension));
    if (domainAssembly != null)
    {
        var domainXmlFilename = $"{domainAssembly.GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, domainXmlFilename));
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var option = new RewriteOptions();
option.AddRedirect("^$", "swagger");

app.UseRewriter(option);

app.Run();

// Exposing Program class for Testing
#pragma warning disable CA1050 // Declare types in namespaces
public partial class Program { }
#pragma warning restore CA1050 // Declare types in namespaces