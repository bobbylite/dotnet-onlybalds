using Microsoft.AspNetCore.SignalR;

namespace OnlyBalds.Hubs;

/// <summary>
/// Represents a SignalR hub for chat functionality.
/// </summary>
public class ChatHub : Hub
{
    /// <summary>
    /// Provides access to methods that can be used from a SignalR hub.
    /// </summary>
    private readonly IHubContext<ChatHub> _hubContext;



    /// <summary>
    /// Initializes a new instance of the <see cref="ChatHub"/> class.
    /// </summary>
    /// <param name="hubContext">The context for the hub, providing access to methods that can be used from a SignalR hub.</param>
    public ChatHub(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Sends a chat message to all connected clients.
    /// </summary>
    /// <param name="user">The user who is sending the message.</param>
    /// <param name="message">The message to send.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SendMessage(string user, string message)
    {
        var username = Context?.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", username, message);
    }

    public override async Task OnConnectedAsync()
    {
        var username = Context?.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", string.Empty, $"{username} joined the OnlyBalds chat room.");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context?.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", string.Empty, $"{username} has left the chat room.");
        await base.OnDisconnectedAsync(exception);
    }
}
