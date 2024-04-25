using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OnlyBalds.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add an HttpClient for the OnlyBalds REST APIs. Includes handling authentication for the API.
builder.AddOnlyBaldsApiClients();

// Add support for authentication and authorization to the application.
builder.AddAccessControl();

await builder.Build().RunAsync();
