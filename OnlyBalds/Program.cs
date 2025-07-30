using OnlyBalds.Components;
using OnlyBalds.Endpoints;
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

// Add support for forwarding HTTP requests to the server.
builder.Services.AddHttpForwarderWithServiceDiscovery();

// Add support for backends for frontends architecture.
builder.AddBff();

// Add authentication and authorization.
builder.AddOnlyBaldsAccessControl();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, 
    // see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFilesOnClient();
app.UseOnlyBaldsAccessControl();
app.UseBffReverseProxy();

app.MapForumEndpoints();
app.MapChatRoomEndpoints();
app.MapMarketplaceEndpoints();
app.MapQuestionnaireEndpoints();

app.MapChatHub();

app.MapGroup("/authentication")
    .MapLoginAndLogout();

app.Run();