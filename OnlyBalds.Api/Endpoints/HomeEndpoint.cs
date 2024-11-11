using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Api.Interfaces.Repositories;

namespace OnlyBalds.Api.Endpoints;

public static class HomeEndpoint
{
    /// <summary>
    /// Maps the endpoints for index page.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapIndexEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", GetIndex)
            .WithName(nameof(GetIndex))
            .WithTags("A Home Endpoint for the index page.")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Returns the index page.";
                operation.Description = "This is a simple way to test the 'Home' endpoint.";
                return operation;
            });
        
        return app;
    }

    private static async Task<IResult> GetIndex([FromServices] IHomeRepository homeRepository)
    {
        ArgumentNullException.ThrowIfNull(homeRepository);

        var index = await homeRepository.GetIndex();
        ArgumentNullException.ThrowIfNull(index);

        return Results.Content(index, "text/html");
    }
}