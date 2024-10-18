using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using OnlyBalds.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add support for exposing API documentation.
builder.AddApiDocumentation();

// Add support for persisting data to a database.
builder.AddDataPersistence();

// Add support for authentication and authorization to the application.
builder.AddAccessControl();

// Configure HTTPS Redirection
builder.Services.Configure<HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = 443; // Set the port to which HTTPS should redirect
});

// Configure Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Enable support for persisting data to a database.
app.UseAccessControl();

// Enable support for exposing API documentation.
app.UseApiDocumentation();

// Use forwarded headers if behind a reverse proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Add middleware for redirecting HTTP Requests to HTTPS.
app.UseHttpsRedirection();

// Maps controllers.
app.MapControllers();

app.Run();