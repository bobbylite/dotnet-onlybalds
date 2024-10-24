using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnlyBalds.Api.Data;
using OnlyBalds.Api.Endpoints;
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
    /// Maps OnlyBalds endpoints.
    /// </summary>
    /// <param name="webApplication"></param>
    /// <returns><see cref="WebApplication"/></returns>
    public static WebApplication MapOnlyBaldsEndpoints(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        webApplication.MapThreadsEndpoints();
        webApplication.MapPostsEndpoints();
        webApplication.MapCommentsEndpoints();

        return webApplication;
    }

    /// <summary>
    /// Run database migrations.
    /// </summary>
    /// <param name="webApplication"></param>
    /// <returns><see cref="WebApplication"/></returns>
    public static WebApplication RunDatabaseMigrations(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        using (var scope = webApplication.Services.CreateScope())
        {
            var threadsDbContext = scope.ServiceProvider.GetRequiredService<OnlyBaldsDataContext>();
            threadsDbContext.Database.Migrate();
        }
        
        return webApplication;
    }

    /// <summary>
    /// Maps the index endpoint for the exposed OnlyBalds API.
    /// </summary>
    /// <param name="webApplication"></param>
    /// <returns><see cref="WebApplication"/></returns>
    public static WebApplication MapHealthChecks(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        webApplication.MapHealthChecks("health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return webApplication;
    }
}