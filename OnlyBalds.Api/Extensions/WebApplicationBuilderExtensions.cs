using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OnlyBalds.Api.Data;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OnlyBalds.Api.Extensions;

/// <summary>
/// Extension methods for <see cref="WebApplicationBuilder"/>.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Add support for persisting data to a database.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static WebApplicationBuilder AddDataPersistence(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        webApplicationBuilder.Services.AddDbContext<ThreadDataContext>(opt => 
            opt.UseInMemoryDatabase("ThreadData"));
        webApplicationBuilder.Services.AddDbContext<PostDataContext>(opt => 
            opt.UseInMemoryDatabase("PostData"));
        webApplicationBuilder.Services.AddDbContext<CommentDataContext>(opt => 
            opt.UseInMemoryDatabase("CommentData"));
        webApplicationBuilder.Services.AddDatabaseDeveloperPageExceptionFilter();
        
        return webApplicationBuilder;
    }
    
    /// <summary>
    /// Add support for API documentation to the application.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// This method adds support for API documentation to the application.
    /// </remarks>
    public static WebApplicationBuilder AddApiDocumentation(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        webApplicationBuilder.Services.AddOptionsWithValidateOnStart<SwaggerOptions>()
            .BindConfiguration(SwaggerOptions.SectionKey);

        webApplicationBuilder.Services.AddEndpointsApiExplorer();
        webApplicationBuilder.Services.AddSwaggerGen(c =>
        {            
            var serviceProvider = webApplicationBuilder.Services.BuildServiceProvider();
            var swaggerOptions = serviceProvider.GetService<IOptionsMonitor<SwaggerOptions>>();

            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Only Balds API", Version = "v1" });

            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(swaggerOptions?.CurrentValue.AuthorizationUrl!),
                        Scopes = new Dictionary<string, string>
                        {
                            { "api://onlybalds/Threads.Read", "Read access to threads api." },
                            { "api://onlybalds/Threads.Write", "Write access to threads api." }
                        }
                    },
                    /*
                    *
                    * Need to figure out how to get this working with the client credentials flow
                    *
                    ClientCredentials = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(swaggerOptions?.CurrentValue.AuthorizationUrl!),
                        TokenUrl = new Uri(swaggerOptions?.CurrentValue.TokenUrl!),
                        //Scopes = swaggerOptions?.CurrentValue.Scopes
                        Scopes = new Dictionary<string, string>
                        {
                            { "api://onlybalds", "Read access to threads api." },
                        }
                    }*/
                }
            });

            // Assign the security requirements to the operations
            c.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        
        return webApplicationBuilder;
    }

    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme {Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"}}
                    ] = new[] {"api://onlybalds"}
                }
            };
        }
    }

    /// <summary>
    /// Add support for authentication and authorization to the application.
    /// With .NET 8, the configuration for this can be controled completely from appsettings.json.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static WebApplicationBuilder AddAccessControl(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        webApplicationBuilder.Services
            .AddAuthentication()
            .AddJwtBearer();
        webApplicationBuilder.Services.AddAuthorization(o =>
        {
            o.AddPolicy("Thread.ReadWrite", p => p.
                RequireAuthenticatedUser().
                RequireClaim("scope", "Threads.Read Threads.Write"));
        });
        
        return webApplicationBuilder;
    }
}