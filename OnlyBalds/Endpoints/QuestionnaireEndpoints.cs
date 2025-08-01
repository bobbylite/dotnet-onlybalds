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

        return endpoints;
    }
}