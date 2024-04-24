using Microsoft.Extensions.Options;
using OnlyBalds.Api;
using OnlyBalds.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add support for exposing API documentation.
builder.AddApiDocumentation();

// Add support for persisting data to a database.
builder.AddDataPersistence();

// Add support for authentication and authorization to the application.
builder.AddAccessControl();

var app = builder.Build();

// Enable support for persisting data to a database.
app.UseAccessControl();

// Configure the HTTP request pipeline.

// Enable support for exposing API documentation.
app.UseApiDocumentation();

// Add middleware for redirecting HTTP Requests to HTTPS.
app.UseHttpsRedirection();

// Maps endpoints for the exposed API.
app.MapThreadsApi();

app.Run();