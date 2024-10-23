using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using OnlyBalds.Api.Endpoints;
using OnlyBalds.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add logging for diagnostics
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add support for exposing API documentation.
builder.AddApiDocumentation();

// Add support for persisting data to a database.
builder.AddDataPersistence();

// Add support for authentication and authorization to the application.
builder.AddAccessControl();

// Add support for repository pattern services.
builder.AddRepositories();

// Configure HTTPS Redirection
builder.Services.Configure<HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = 443; // Set the port to which HTTPS should redirect
});

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

// Maps the index endpoint for the exposed OnlyBalds API.
app.MapIndexEndpoint();

// Maps endpoints for the exposed OnlyBalds API.
app.MapOnlyBaldsEndpoints();

app.Run();