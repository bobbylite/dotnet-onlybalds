using OnlyBalds.Components;
using OnlyBalds.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the application.
builder.AddServices();

// Add Hugging Face Inference services.
builder.AddHuggingFaceInferenceServices();

// Add the token client.
builder.AddTokenClient();

// Add the Hugging Face Inference API client.
builder.AddOnlyBaldsApiClients();

// Add the Inference API client.
builder.AddInferenceApiClients();

// Add authentication and authorization.
builder.AddAccessControl();

// Add support for forwarding HTTP requests to the server.
builder.Services.AddHttpForwarderWithServiceDiscovery();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(OnlyBalds.Client.Components._Imports).Assembly);

app.MapOnlyBaldsApiProxy();

app.MapChatHub();

app.MapGroup("/authentication")
    .MapLoginAndLogout();

app.Run();