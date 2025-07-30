namespace OnlyBalds.Endpoints;

/// <summary>
/// Provides extension methods for mapping OnlyBalds forum endpoints.
/// </summary>
public static class ChatRoomEndpoints
{
    /// <summary>
    /// Extension methods for mapping OnlyBalds chat room endpoints.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapChatRoomEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapGet("/chat-room", context =>
        {
            context.Response.ContentType = "text/html";
            context.Response.Redirect("/chat-room.html");

            return Task.CompletedTask;
        }).RequireAuthorization();

        return endpoints;
    }
}