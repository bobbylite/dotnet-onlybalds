using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Api.Constants;
using OnlyBalds.Api.Extensions;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;

namespace OnlyBalds.Api.Endpoints;

/// <summary>
/// Collection of endpoints for managing accounts.
/// </summary>
public static class AccountEndpoints
{
    /// <summary>
    /// Map accounts endpoints to the application.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapAccountsEndpoints(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapGet("/accounts", GetAccountsAsync)
            .WithName(nameof(GetAccountsAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPost("/accounts", CreateAccountAsync)
            .WithName(nameof(CreateAccountAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPatch("/accounts", PatchAccountAsync)
            .WithName(nameof(PatchAccountAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapDelete("/accounts", DeleteAccountAsync)
            .WithName(nameof(DeleteAccountAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        return app;
    }

    /// <summary>
    /// Retrieves all accounts from the repository.
    /// </summary>
    /// <param name="accountsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> GetAccountsAsync(
        string? accountId,
        [FromServices] IOnlyBaldsRepository<Account> accountsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(accountsRepository);

        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var accessToken = await httpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return Results.BadRequest("Could not retrieve access token.");
        }

        var isAuthorized = await httpContext.IsAuthorizedUserAsync(accessToken);
        var isAuthorizedAdmin = await httpContext.IsAuthorizedAdminAsync(accessToken);
        var userId = await httpContext.GetUserIdAsync(accessToken);

        if (isAuthorized is false || string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        if (string.IsNullOrEmpty(accountId) is not true)
        {
            var account = accountsRepository.GetById(Guid.Parse(accountId));

            if (account.IdentityProviderId.Equals(userId, StringComparison.OrdinalIgnoreCase) is false)
            {
                if (isAuthorizedAdmin is false)
                {
                    return Results.Unauthorized();
                }
            }

            return Results.Ok(account);
        }

        if (isAuthorizedAdmin is false)
        {
            return Results.Ok(accountsRepository
                .GetAll()
                .Where(a => a.IdentityProviderId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                .ToList());
        }

        return Results.Ok(accountsRepository.GetAll());
    }

    /// <summary>
    /// Creates a new account.
    /// </summary>
    /// <param name="accountItem"></param>
    /// <param name="accountsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> CreateAccountAsync(
        [FromBody] Account accountItem,
        [FromServices] IOnlyBaldsRepository<Account> accountsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(accountItem);
        ArgumentNullException.ThrowIfNull(accountsRepository);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var accessToken = await httpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return Results.BadRequest("Could not retrieve access token.");
        }

        var isAuthorized = await httpContext.IsAuthorizedUserAsync(accessToken);
        var isAuthorizedAdmin = await httpContext.IsAuthorizedAdminAsync(accessToken);
        var userId = await httpContext.GetUserIdAsync(accessToken);

        if (isAuthorized is false || string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        if (accountItem.IdentityProviderId.Equals(userId, StringComparison.OrdinalIgnoreCase) is false)
        {
            if (isAuthorizedAdmin is false)
            {
                return Results.Unauthorized();
            }
        }

        await accountsRepository.Add(accountItem);

        return Results.Created($"/accounts?accountId={accountItem.Id}", accountItem);
    }

    /// <summary>
    /// Updates an existing account with new data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="accountItem"></param>
    /// <param name="accountsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> PatchAccountAsync(
        string? accountId,
        [FromBody] Account accountItem,
        [FromServices] IOnlyBaldsRepository<Account> accountsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(accountsRepository);

        if (accountItem is null)
        {
            return Results.BadRequest("Account item cannot be null.");
        }

        if (string.IsNullOrEmpty(accountId))
        {
            return Results.BadRequest("Account ID cannot be null or empty.");
        }

        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var accessToken = await httpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return Results.BadRequest("Could not retrieve access token.");
        }

        var isAuthorized = await httpContext.IsAuthorizedUserAsync(accessToken);
        var isAuthorizedAdmin = await httpContext.IsAuthorizedAdminAsync(accessToken);
        var userId = await httpContext.GetUserIdAsync(accessToken);

        if (isAuthorized is false || string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        var account = accountsRepository.GetById(Guid.Parse(accountId));
        ArgumentNullException.ThrowIfNull(account);

        if (account.IdentityProviderId.Equals(userId, StringComparison.OrdinalIgnoreCase) is false)
        {
            if (isAuthorizedAdmin is false)
            {
                return Results.Unauthorized();
            }
        }

        account.Email = string.IsNullOrEmpty(accountItem.Email) ? account.Email : accountItem.Email;
        account.Address = string.IsNullOrEmpty(accountItem.Address) ? account.Address : accountItem.Address;
        account.DisplayName = string.IsNullOrEmpty(accountItem.DisplayName) ? account.DisplayName : accountItem.DisplayName;
        account.FirstName = string.IsNullOrEmpty(accountItem.FirstName) ? account.FirstName : accountItem.FirstName;
        account.LastName = string.IsNullOrEmpty(accountItem.LastName) ? account.LastName : accountItem.LastName;

        await accountsRepository.UpdateById(Guid.Parse(accountId));

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes account by account id from the repository.
    /// </summary>
    /// <param name="accountsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeleteAccountAsync(
        string? accountId,
        [FromServices] IOnlyBaldsRepository<Account> accountsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(accountsRepository);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        if (string.IsNullOrEmpty(accountId))
        {
            return Results.BadRequest("Account ID cannot be null or empty.");
        }

        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var accessToken = await httpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return Results.BadRequest("Could not retrieve access token.");
        }

        var isAuthorized = await httpContext.IsAuthorizedUserAsync(accessToken);
        var isAuthorizedAdmin = await httpContext.IsAuthorizedAdminAsync(accessToken);
        var userId = await httpContext.GetUserIdAsync(accessToken);

        if (string.IsNullOrEmpty(userId) || isAuthorized is false)
        {
            return Results.Unauthorized();
        }

        var id = Guid.Parse(accountId);
        var account = accountsRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(account);

        if (account.IdentityProviderId.Equals(userId, StringComparison.OrdinalIgnoreCase) is false)
        {
            if (isAuthorizedAdmin is false)
            {
                return Results.Unauthorized();
            }
        }

        await accountsRepository.DeleteById(id);

        return Results.NoContent();
    }
}
