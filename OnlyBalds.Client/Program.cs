using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OnlyBalds.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add an HttpClient for the Threads REST API. Includes handling authentication for the API.
builder.AddThreadsApiClient();

// Add support for authentication and authorization to the application.
builder.AddAccessControl();

await builder.Build().RunAsync();
