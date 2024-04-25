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
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public override async Task OnConnectedAsync()
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "OnlyBalds Chat AI", "Welcome to the OnlyBalds chat room!");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Handle the disconnection here
        // For example, you might want to send a message to all clients to let them know that a user has disconnected

        await base.OnDisconnectedAsync(exception);
    }
}
