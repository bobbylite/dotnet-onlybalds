using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OnlyBalds.Api.Data;
using OnlyBalds.Api.Health;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;
using OnlyBalds.Api.Repositories;
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
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Only Balds API",
                Version = "v1"
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter 'Bearer' followed by your JWT token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            c.AddSecurityDefinition("Bearer", securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    securityScheme,
                    Array.Empty<string>()
                }
            };

            c.AddSecurityRequirement(securityRequirement);
        });

        return webApplicationBuilder;
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
            o.AddPolicy(AuthorizationPolicies.UserAccess, p =>
                p.RequireClaim(AuthorizationClaims.Permissions, AuthorizationPermissions.UserAccess));
            o.AddPolicy(AuthorizationPolicies.AdminAccess, p =>
                p.RequireClaim(AuthorizationClaims.Permissions, AuthorizationPermissions.AdminAccess));
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