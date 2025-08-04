using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OnlyBalds.Api.Data;
using OnlyBalds.Api.Health;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;
using OnlyBalds.Api.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;
using OnlyBalds.Api.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace OnlyBalds.Api.Extensions;

/// <summary>
/// Extension methods for <see cref="WebApplicationBuilder"/>.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Add support for persisting data to a PostgreSQL database in Azure.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static WebApplicationBuilder AddDataPersistence(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);
        
        var connectionString = webApplicationBuilder.Configuration.GetConnectionString("PostgreSqlConnection");

        webApplicationBuilder.Services.AddDbContext<OnlyBaldsDataContext>(opt => 
            opt.UseNpgsql(connectionString));
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

            // Configure OAuth2 with Authorization Code Flow (PKCE)
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(swaggerOptions?.CurrentValue.AuthorizationUrl!),
                        TokenUrl = new Uri(swaggerOptions?.CurrentValue.TokenUrl!),
                        Scopes = new Dictionary<string, string>
                        {
                            { "Threads.Read", "Read access to threads API" },
                            { "Threads.Write", "Write access to threads API" }
                        },
                    }
                },
            });

            c.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        return webApplicationBuilder;
    }

    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Ensure the security requirements are added to each operation
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        }
                    ] = new[] { "api://onlybalds/Threads.Read", "api://onlybalds/Threads.Write" }
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
            o.AddPolicy(AuthorizataionPolicyNames.UserAccess, p => p.
                RequireClaim("permissions", AuthorizataionPolicyNames.UserAccess));
            o.AddPolicy(AuthorizataionPolicyNames.AdminAccess, p => p.
                RequireClaim("permissions", AuthorizataionPolicyNames.AdminAccess));
        });

        webApplicationBuilder.Services.AddHttpContextAccessor();

        webApplicationBuilder.Services.PostConfigure<JwtBearerOptions>("Bearer", options =>
        {
            options.Events ??= new JwtBearerEvents();

            options.Events.OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtAuth");

                logger.LogWarning("JWT authentication failed: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            };

            options.Events.OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtAuth");

                var claims = context.Principal?.Claims
                    .Select(c => new { c.Type, c.Value })
                    .ToList();
                logger.LogInformation("Claims: {Claims}", claims);
                return Task.CompletedTask;
            };

            options.Events.OnForbidden = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtAuth");

                logger.LogWarning("Forbidden access: {Message}", context.Response.StatusCode);
                return Task.CompletedTask;
            };
        });

        
        return webApplicationBuilder;
    }

    /// <summary>
    /// Add support for repository pattern services.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        webApplicationBuilder.Services.AddScoped<IHomeRepository, HomeRepository>();
        webApplicationBuilder.Services.AddScoped<IOnlyBaldsRepository<ThreadItem>, OnlyBaldsRepository<ThreadItem>>();
        webApplicationBuilder.Services.AddScoped<IOnlyBaldsRepository<PostItem>, OnlyBaldsRepository<PostItem>>();
        webApplicationBuilder.Services.AddScoped<IOnlyBaldsRepository<CommentItem>, OnlyBaldsRepository<CommentItem>>();
        webApplicationBuilder.Services.AddScoped<IOnlyBaldsRepository<QuestionnaireItems>, OnlyBaldsRepository<QuestionnaireItems>>();
        webApplicationBuilder.Services.AddScoped<IOnlyBaldsRepository<Account>, OnlyBaldsRepository<Account>>();

        return webApplicationBuilder;
    }

    /// <summary>
    /// Add health checks to the application.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        var connectionString = webApplicationBuilder.Configuration.GetConnectionString("PostgreSqlConnection");

        webApplicationBuilder.Services.AddHealthChecks()
            .AddCheck<OnlyBaldsDatabaseHealthCheck>("OnlyBalds Database Tables Health Check", HealthStatus.Unhealthy)
            .AddDbContextCheck<OnlyBaldsDataContext>("ADO.NET DbContext Health Check", HealthStatus.Unhealthy)
            .AddNpgSql(connectionString!);

        return webApplicationBuilder;
    }
}