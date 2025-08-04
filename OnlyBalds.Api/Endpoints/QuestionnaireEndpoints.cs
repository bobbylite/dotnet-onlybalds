using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Api.Constants;
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
        app.MapGet("/questionnaire", GetQuestionnaires)
            .WithName(nameof(GetQuestionnaires))
            .WithOpenApi()
            .RequireAuthorization(AuthorizataionPolicyNames.UserAccess);

        app.MapGet("/questionnaire/{id}", GetQuestionnaireById)
            .WithName(nameof(GetQuestionnaireById))
            .WithOpenApi()
            .RequireAuthorization(AuthorizataionPolicyNames.UserAccess);

        app.MapPost("/questionnaire", CreateQuestionnaireAsync)
            .WithName(nameof(CreateQuestionnaireAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizataionPolicyNames.UserAccess);

        app.MapPut("/questionnaire/{id}", UpdateQuestionnaireAsync)
            .WithName(nameof(UpdateQuestionnaireAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizataionPolicyNames.UserAccess);

        app.MapDelete("/questionnaire/{id}", DeleteQuestionnaireAsync)
            .WithName(nameof(DeleteQuestionnaireAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizataionPolicyNames.UserAccess);

        return app;
    }

    /// <summary>
    /// Retrieves all questionnaire from the repository.
    /// </summary>
    /// <param name="questionnaireRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetQuestionnaires([FromServices] IOnlyBaldsRepository<QuestionnaireItems> questionnaireRepository)
    {
        ArgumentNullException.ThrowIfNull(questionnaireRepository);

        var questionnaires = questionnaireRepository.GetAll();
        ArgumentNullException.ThrowIfNull(questionnaires);

        return Results.Ok(questionnaires);
    }

    /// <summary>
    /// Retrieves a specific questionnaire by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="questionnaireRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetQuestionnaireById(Guid id, [FromServices] IOnlyBaldsRepository<QuestionnaireItems> questionnaireRepository)
    {
        ArgumentNullException.ThrowIfNull(questionnaireRepository);

        var questionnaire = questionnaireRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(questionnaire);

        return Results.Ok(questionnaire);
    }

    /// <summary>
    /// Creates a new questionnaire.
    /// </summary>
    /// <param name="questionnaireItems"></param>
    /// <param name="questionnaireRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> CreateQuestionnaireAsync(
        [FromBody] QuestionnaireItems questionnaireItems,
        [FromServices] IOnlyBaldsRepository<QuestionnaireItems> questionnaireRepository)
    {
        ArgumentNullException.ThrowIfNull(questionnaireItems);
        ArgumentNullException.ThrowIfNull(questionnaireRepository);

        if (questionnaireItems.Id == Guid.Empty)
        {
            questionnaireItems.Id = Guid.NewGuid();
        }

        if (questionnaireItems.Data?.Id == Guid.Empty)
        {
            questionnaireItems.Data.Id = Guid.NewGuid();
        }

        foreach(var baldingOption in questionnaireItems.Data?.BaldingOptions!)
        {
            if (baldingOption.Id == Guid.Empty)
            {
                baldingOption.Id = Guid.NewGuid();
            }

            foreach(var option in baldingOption.Option!)
            {
                if (option.Id == Guid.Empty)
                {
                    option.Id = Guid.NewGuid();
                }

                foreach(var question in option.Questions!)
                {
                    if (question.Id == Guid.Empty)
                    {
                        question.Id = Guid.NewGuid();
                    }
                }
            }
        }

        foreach(var question in questionnaireItems.Data?.Questions!)
        {
            if (question.Id == Guid.Empty)
            {
                question.Id = Guid.NewGuid();
            }
        }

        questionnaireItems.StartDate = DateTime.UtcNow.ToUniversalTime();

        await questionnaireRepository.Add(questionnaireItems);

        return Results.Created($"/Questionnaires/{questionnaireItems.Id}", questionnaireItems);
    }

    /// <summary>
    /// Updates an existing questionnaire with new data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="questionnaireItems"></param>
    /// <param name="questionnaireRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> UpdateQuestionnaireAsync(Guid id, 
        [FromBody] QuestionnaireItems questionnaireItems,
        [FromServices] IOnlyBaldsRepository<QuestionnaireItems> questionnaireRepository)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(questionnaireItems);
        ArgumentNullException.ThrowIfNull(questionnaireRepository);

        var questionnaire = questionnaireRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(questionnaire);

        questionnaire.Data = questionnaireItems.Data;

        await questionnaireRepository.UpdateById(id);

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes a questionnaire by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="questionnaireRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeleteQuestionnaireAsync(Guid id,
        [FromServices] IOnlyBaldsRepository<QuestionnaireItems> questionnaireRepository)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(questionnaireRepository);

        await questionnaireRepository.DeleteById(id);

        return Results.NoContent();
    }
}