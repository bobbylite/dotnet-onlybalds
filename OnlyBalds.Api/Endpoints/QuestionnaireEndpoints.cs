using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyBalds.Api.Constants;
using OnlyBalds.Api.Extensions;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;

namespace OnlyBalds.Api.Endpoints;

/// <summary>
/// Represents the endpoints for the questionnaire api.
/// </summary>
public static class QuestionnaireEndpoints
{
    /// <summary>
    /// Maps the endpoints for the questionnaire api.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapQuestionnaireEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/questionnaire", GetQuestionnairesAsync)
            .WithName(nameof(GetQuestionnairesAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPost("/questionnaire", CreateQuestionnaireAsync)
            .WithName(nameof(CreateQuestionnaireAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapDelete("/questionnaire", DeleteQuestionnaireAsync)
            .WithName(nameof(DeleteQuestionnaireAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        return app;
    }

    /// <summary>
    /// Retrieves all questionnaires from the repository.
    /// </summary>
    /// <param name="questionnaireRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> GetQuestionnairesAsync(
        string? questionnaireId,
        [FromServices] IOnlyBaldsRepository<QuestionnaireItems> questionnaireRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(questionnaireRepository);

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

        if (string.IsNullOrEmpty(questionnaireId) is not true)
        {
            var questionnaire = questionnaireRepository
                .GetDbSet()
                .Include(q => q.Data)
                .Where(q => q.Id == Guid.Parse(questionnaireId))
                .SingleOrDefault();

            if (questionnaire is null)
            {
                return Results.NotFound();
            }

            if (questionnaire.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) is false)
            {
                if (isAuthorizedAdmin is false)
                {
                    return Results.Unauthorized();
                }
            }

            return Results.Ok(questionnaire);
        }

        if (isAuthorizedAdmin is false)
        {
            return Results.Ok(questionnaireRepository
                .GetDbSet()
                .Include(q => q.Data)
                .Where(a => a.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                .ToList());
        }

        return Results.Ok(questionnaireRepository.GetDbSet().Include(q => q.Data).ToList());
    }

    /// <summary>
    /// Creates a new questionnaire.
    /// </summary>
    /// <param name="questionnaireItems"></param>
    /// <param name="questionnaireRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> CreateQuestionnaireAsync(
        [FromBody] QuestionnaireItems questionnaireItems,
        [FromServices] IOnlyBaldsRepository<QuestionnaireItems> questionnaireRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(questionnaireItems);
        ArgumentNullException.ThrowIfNull(questionnaireRepository);

        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var accessToken = await httpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return Results.BadRequest("Could not retrieve access token.");
        }

        var userId = await httpContext.GetUserIdAsync(accessToken);

        if (questionnaireItems.UserId != userId)
        {
            return Results.BadRequest("User ID does not match the authenticated user.");
        }

        if (string.IsNullOrEmpty(questionnaireItems.UserId))
        {
            questionnaireItems.UserId = userId;
        }

        if (questionnaireItems.Id == Guid.Empty)
        {
            questionnaireItems.Id = Guid.NewGuid();
        }

        if (questionnaireItems.Data != null && questionnaireItems.Data.Id == Guid.Empty)
        {
            questionnaireItems.Data.Id = Guid.NewGuid();
        }

        questionnaireItems.StartDate = DateTime.UtcNow;

        await questionnaireRepository.Add(questionnaireItems);

        return Results.Created($"/Questionnaires/{questionnaireItems.Id}", questionnaireItems);
    }


    /// <summary>
    /// Deletes a questionnaire by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="questionnaireRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeleteQuestionnaireAsync(
        string? questionnaireId,
        [FromServices] IOnlyBaldsRepository<QuestionnaireItems> questionnaireRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(questionnaireRepository);

        if (string.IsNullOrEmpty(questionnaireId))
        {
            return Results.BadRequest("Questionnaire ID cannot be empty.");
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

        var id = Guid.Parse(questionnaireId);
        var questionnaire = questionnaireRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(questionnaire);

        if (questionnaire.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) is false)
        {
            if (isAuthorizedAdmin is false)
            {
                return Results.Unauthorized();
            }
        }

        await questionnaireRepository.DeleteById(id);

        return Results.NoContent();
    }
}
