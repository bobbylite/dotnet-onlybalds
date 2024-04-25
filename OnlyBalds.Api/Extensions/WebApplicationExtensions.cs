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

        webApplication.MapPut("/threads/{id}", async (Guid id, ThreadItem content, ThreadDataContext dataContext) =>
            {
                var threadItem = await dataContext.ThreadItems.FindAsync(id);

                if (threadItem is null) return Results.NotFound();

                threadItem.Name = content.Name;
                threadItem.Title = content.Title;
                threadItem.Summary = content.Summary;
                threadItem.Creator = content.Creator;
                threadItem.StartDate = content.StartDate;
                threadItem.PostsCount = content.PostsCount;

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

    /// <summary>
    /// Maps endpoints for the exposed API.
    /// </summary>
    /// <param name="webApplication"></param>
    public static WebApplication MapPostsApi(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        webApplication.MapGet("/posts", async (PostDataContext dataContext) =>
                await dataContext.PostItems.ToListAsync())
            .WithName("GetPosts")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

            webApplication.MapGet("/posts/{id}", async (Guid id, PostDataContext dataContext) =>
                await dataContext.PostItems.FindAsync(id)
                    is { } taskItem
                    ? Results.Ok(taskItem)
                    : Results.NotFound())
            .WithName("GetPostById")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");
        
        webApplication.MapPost("/posts", async (PostItem postItem, PostDataContext dataContext) =>
            {
                if (postItem.Id == Guid.Empty)
                {
                    postItem.Id = Guid.NewGuid();
                }

                dataContext.PostItems.Add(postItem);
                await dataContext.SaveChangesAsync();

                return Results.Created($"/posts/{postItem.Id}", postItem);
            })
            .WithName("CreatePost")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

        webApplication.MapPut("/posts/{id}", async (Guid id, PostItem content, PostDataContext dataContext) =>
            {
                var postItem = await dataContext.PostItems.FindAsync(id);

                if (postItem is null)
                {
                    return Results.NotFound();
                }

                postItem.Title = content.Title;
                postItem.Content = content.Content;

                await dataContext.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("UpdatePost")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

        webApplication.MapDelete("/posts/{id}", async (Guid id, PostDataContext dataContext) =>
            {
                if (await dataContext.PostItems.FindAsync(id) is { } taskItem)
                {
                    dataContext.PostItems.Remove(taskItem);
                    await dataContext.SaveChangesAsync();
                    return Results.NoContent();
                }

                return Results.NotFound();
            })
            .WithName("DeletePost")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

        return webApplication;
    }

    /// <summary>
    /// Maps endpoints for the exposed API.
    /// </summary>
    /// <param name="webApplication"></param>
    public static WebApplication MapCommentsApi(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        webApplication.MapGet("/comments", async (CommentDataContext dataContext) =>
                await dataContext.CommentItems.ToListAsync())
            .WithName("GetComment")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

            webApplication.MapGet("/comments/{id}", async (Guid id, CommentDataContext dataContext) =>
                await dataContext.CommentItems.FindAsync(id)
                    is { } commentItem
                    ? Results.Ok(commentItem)
                    : Results.NotFound())
            .WithName("GetCommentById")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");
        
        webApplication.MapPost("/comments", async (CommentItem commentItem, CommentDataContext dataContext) =>
            {
                if (commentItem.Id == Guid.Empty)
                {
                    commentItem.Id = Guid.NewGuid();
                }

                dataContext.CommentItems.Add(commentItem);
                await dataContext.SaveChangesAsync();

                return Results.Created($"/comments/{commentItem.Id}", commentItem);
            })
            .WithName("CreateComment")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

        webApplication.MapPut("/comments/{id}", async (Guid id, PostItem content, CommentDataContext dataContext) =>
            {
                var commentItem = await dataContext.CommentItems.FindAsync(id);

                if (commentItem is null)
                {
                    return Results.NotFound();
                }

                commentItem.Content = content.Content;

                await dataContext.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("UpdateComment")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

        webApplication.MapDelete("/comments/{id}", async (Guid id, CommentDataContext dataContext) =>
            {
                if (await dataContext.CommentItems.FindAsync(id) is { } taskItem)
                {
                    dataContext.CommentItems.Remove(taskItem);
                    await dataContext.SaveChangesAsync();
                    return Results.NoContent();
                }

                return Results.NotFound();
            })
            .WithName("DeleteComment")
            .WithOpenApi()
            .RequireAuthorization("Thread.ReadWrite");

        return webApplication;
    }
}
