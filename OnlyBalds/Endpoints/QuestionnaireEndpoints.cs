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

        endpoints.MapGet("/questionnaire", context =>
        {
            context.Response.ContentType = "text/html";
            context.Response.Redirect("/questionnaire.html");

            return Task.CompletedTask;
        }).RequireAuthorization();

        return endpoints;
    }
}