using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyBalds.Api.Constants;
using OnlyBalds.Api.Data;
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

        app.MapGet("/account", GetAccountByIdAsync)
            .WithName(nameof(GetAccountByIdAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPost("/account", CreateAccountAsync)
            .WithName(nameof(CreateAccountAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPut("/account/{id}", UpdateAccountAsync)
            .WithName(nameof(UpdateAccountAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapDelete("/account/{id}", DeleteAccountAsync)
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
    public static IResult GetAccountsAsync([FromServices] IOnlyBaldsRepository<Account> accountsRepository)
    {
        ArgumentNullException.ThrowIfNull(accountsRepository);

        var accounts = accountsRepository.GetAll();
        ArgumentNullException.ThrowIfNull(accounts);

        return Results.Ok(accounts);
    }

    /// <summary>
    /// Retrieves accounts that match the id from the repository.
    /// </summary>
    /// <param name="accountsRepository"></param>
    /// <param name="id"></param>
    public static IResult GetAccountByIdAsync(
        [FromServices] OnlyBaldsDataContext context,
        [FromQuery] string id)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(id);

        var accountsRepository = context.Set<Account>();
        var account = accountsRepository.SingleOrDefault(x => x.IdentityProviderId == id);

        if (account is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(account);
    }

    /// <summary>
    /// Creates a new account.
    /// </summary>
    /// <param name="accountItem"></param>
    /// <param name="accountsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> CreateAccountAsync([FromBody] Account accountItem, [FromServices] IOnlyBaldsRepository<Account> accountsRepository)
    {
        ArgumentNullException.ThrowIfNull(accountItem);
        ArgumentNullException.ThrowIfNull(accountsRepository);

        if (accountItem.Id == Guid.Empty)
        {
            accountItem.Id = Guid.NewGuid();
        }

        await accountsRepository.Add(accountItem);

        return Results.Created($"/accounts/{accountItem.Id}", accountItem);
    }

    /// <summary>
    /// Updates an existing account with new data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="accountItem"></param>
    /// <param name="accountsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> UpdateAccountAsync(Guid id, [FromBody] Account accountItem, [FromServices] IOnlyBaldsRepository<Account> accountsRepository)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(accountItem);
        ArgumentNullException.ThrowIfNull(accountsRepository);

        var account = accountsRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(account);

        account.Email = accountItem.Email;
        account.Username = accountItem.Username;
        account.HasSubmittedQuistionnaire = accountItem.HasSubmittedQuistionnaire;
        account.QuestionnaireId = accountItem.QuestionnaireId;
        account.IdentityProviderId = accountItem.IdentityProviderId;

        await accountsRepository.UpdateById(id);

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes a account by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="accountsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeleteAccountAsync(Guid id, [FromServices] IOnlyBaldsRepository<Account> accountsRepository)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(accountsRepository);

        await accountsRepository.DeleteById(id);

        return Results.NoContent();
    }
}