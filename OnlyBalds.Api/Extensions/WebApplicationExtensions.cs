using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnlyBalds.Api.Data;
using OnlyBalds.Api.Models;

namespace OnlyBalds.Api.Extensions;

/// <summary>
/// Extension methods for <see cref="WebApplication"/>.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Enable support for persisting data to a database.
    /// </summary>
    /// <param name="webApplication"></param>
    public static WebApplication UseAccessControl(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);
        
        webApplication.UseAuthentication();
        webApplication.UseAuthorization();
        
        return webApplication;
    }
    
    /// <summary>
    /// Enable support for exposing API documentation.
    /// </summary>
    /// <param name="webApplication"></param>
    public static WebApplication UseApiDocumentation(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        var optionsMonitor = webApplication.Services.GetRequiredService<IOptionsMonitor<SwaggerOptions>>();
        var swaggerOptions = optionsMonitor.CurrentValue;
        
        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.UseSwagger();
            webApplication.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerOptions.Endpoint, swaggerOptions.APIName);
                c.OAuth2RedirectUrl(swaggerOptions.OAuth2RedirectUrl);
                c.OAuthClientId(swaggerOptions.OAuthClientId);
                c.OAuthClientSecret(swaggerOptions.OAuthClientSecret);
                c.OAuthRealm(swaggerOptions.OAuthRealm);
                c.OAuthAppName(swaggerOptions.OAuthAppName);
                if (swaggerOptions.OAuthUsePkce)
                {
                    c.OAuthUsePkce();
                }
            });
        }
        
        return webApplication;
    }
    
    
    /// <summary>
    /// Maps endpoints for the exposed API.
    /// </summary>
    /// <param name="webApplication"></param>
    public static WebApplication MapThreadsApi(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        webApplication.MapGet("/threads", async (ThreadDataContext dataContext) =>
                await dataContext.ThreadItems.ToListAsync())
            .WithName("GetThreads")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

            webApplication.MapGet("/threads/{id}", async (Guid id, ThreadDataContext dataContext) =>
                await dataContext.ThreadItems.FindAsync(id)
                    is { } taskItem
                    ? Results.Ok(taskItem)
                    : Results.NotFound())
            .WithName("GetThreadById")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");
        
        webApplication.MapPost("/threads", async (ThreadItem threadItem, ThreadDataContext dataContext) =>
            {
                if (threadItem.Id == Guid.Empty)
                {
                    threadItem.Id = Guid.NewGuid();
                }

                dataContext.ThreadItems.Add(threadItem);
                await dataContext.SaveChangesAsync();

                return Results.Created($"/threads/{threadItem.Id}", threadItem);
            })
            .WithName("CreateThread")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

        webApplication.MapPut("/threads/{id}", async (Guid id, ThreadItem inputTodo, ThreadDataContext dataContext) =>
            {
                var threadItem = await dataContext.ThreadItems.FindAsync(id);

                if (threadItem is null) return Results.NotFound();

                threadItem.Name = inputTodo.Name;
                threadItem.Title = inputTodo.Title;
                threadItem.Summary = inputTodo.Summary;
                threadItem.Creator = inputTodo.Creator;
                threadItem.StartDate = inputTodo.StartDate;



                await dataContext.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("UpdateThread")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

        webApplication.MapDelete("/threads/{id}", async (Guid id, ThreadDataContext dataContext) =>
            {
                if (await dataContext.ThreadItems.FindAsync(id) is { } taskItem)
                {
                    dataContext.ThreadItems.Remove(taskItem);
                    await dataContext.SaveChangesAsync();
                    return Results.NoContent();
                }

                return Results.NotFound();
            })
            .WithName("DeleteThread")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

        return webApplication;
    }
}
