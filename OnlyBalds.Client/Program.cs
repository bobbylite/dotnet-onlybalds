using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OnlyBalds.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add an HttpClient for the Tasks REST API. Includes handling authentication for the API.
builder.AddTasksApiClient();

// Add support for authentication and authorization to the application.
builder.AddAccessControl();

await builder.Build().RunAsync();
