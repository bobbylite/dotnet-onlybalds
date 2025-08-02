using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Client.Models;

namespace OnlyBalds.Endpoints;

/// <summary>
/// Provides extension methods for mapping OnlyBalds forum endpoints.
/// </summary>
public static class QuestionnaireEndpoints
{
    /// <summary>
    /// Extension methods for mapping OnlyBalds questionnaire endpoints.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapQuestionnaireEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapGet("/account-questionnaire", context =>
        {
            var isAuthenticated = context.User?.Identity?.IsAuthenticated;

            if (isAuthenticated is null || isAuthenticated is false)
            {
                context.Response.ContentType = "text/html";
                context.Response.Redirect("/access-denied.html");
                return Task.CompletedTask;
            }

            context.Response.ContentType = "text/html";
            context.Response.Redirect("/questionnaire.html");
            return Task.CompletedTask;
        }).AllowAnonymous();

        endpoints
        .MapPost("/account-questionnaire", PostQuestionnaire)
        .RequireAuthorization();

        return endpoints;
    }

    private static async Task PostQuestionnaire(
        [FromBody] QuestionnaireItems questionnaireItem,
        HttpContext context,
        [FromServices] IHttpClientFactory httpClientFactory
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(questionnaireItem);

        var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBalds);
        var response = await httpClient.PostAsJsonAsync("questionnaire", questionnaireItem);

        if (questionnaireItem is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        
        context.Response.Redirect("/");
    }
}