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
    public static WebApplication MapApi(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        webApplication.MapGet("/tasks", async (TaskDataContext dataContext) =>
                await dataContext.TaskItems.ToListAsync())
            .WithName("GetTasks")
            .WithOpenApi()
            .RequireAuthorization("Tasks.Read");

        webApplication.MapGet("/tasks/complete", async (TaskDataContext dataContext) =>
                await dataContext.TaskItems.Where(t => t.IsComplete).ToListAsync())
            .WithName("GetCompletedTasks")
            .WithOpenApi()
            .RequireAuthorization("Tasks.Read");

        webApplication.MapGet("/tasks/{id}", async (Guid id, TaskDataContext dataContext) =>
                await dataContext.TaskItems.FindAsync(id)
                    is { } taskItem
                    ? Results.Ok(taskItem)
                    : Results.NotFound())
            .WithName("GetTaskById")
            .WithOpenApi()
            .RequireAuthorization("Tasks.Read");

        webApplication.MapPost("/tasks", async (TaskItem taskItem, TaskDataContext dataContext) =>
            {
                if (taskItem.Id == Guid.Empty)
                {
                    taskItem.Id = Guid.NewGuid();
                }

                dataContext.TaskItems.Add(taskItem);
                await dataContext.SaveChangesAsync();

                return Results.Created($"/tasks/{taskItem.Id}", taskItem);
            })
            .WithName("CreateTask")
            .WithOpenApi()
            .RequireAuthorization("Tasks.Read");

        webApplication.MapPut("/tasks/{id}", async (Guid id, TaskItem inputTodo, TaskDataContext dataContext) =>
            {
                var taskItem = await dataContext.TaskItems.FindAsync(id);

                if (taskItem is null) return Results.NotFound();

                taskItem.Name = inputTodo.Name;
                taskItem.IsComplete = inputTodo.IsComplete;

                await dataContext.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("UpdateTask")
            .WithOpenApi()
            .RequireAuthorization("Tasks.Read");

        webApplication.MapDelete("/tasks/{id}", async (Guid id, TaskDataContext dataContext) =>
            {
                if (await dataContext.TaskItems.FindAsync(id) is { } taskItem)
                {
                    dataContext.TaskItems.Remove(taskItem);
                    await dataContext.SaveChangesAsync();
                    return Results.NoContent();
                }

                return Results.NotFound();
            })
            .WithName("DeleteTask")
            .WithOpenApi()
            .RequireAuthorization("Tasks.Read");

        return webApplication;
    }
}
