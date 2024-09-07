using Microsoft.AspNetCore.SignalR;

namespace OnlyBalds.Hubs;

/// <summary>
/// Represents a SignalR hub for global chat features.
/// </summary>
public class ChatHub : Hub
{
    // <summary>
    // The context for the hub, providing access to methods that can be used from a SignalR hub.
    // </summary>
    private readonly IHubContext<ChatHub> _hubContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatHub"/> class.
    /// </summary>
    /// <param name="hubContext">The context for the hub.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="ChatHub"/> class.
    /// </remarks>
    public ChatHub(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Sends a message to all clients.
    /// </summary>
    /// <param name="user">The user sending the message.</param>
    /// <param name="message">The message to send.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task SendMessage(string user, string message)
    {
        var username = Context?.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", username, message);
    }

    /// <summary>
    /// Invoked when a new connection is established with the hub.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is called when a new connection is established with the hub.
    /// </remarks>
    public override async Task OnConnectedAsync()
    {
        var username = Context?.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", string.Empty, $"{username} joined the OnlyBalds chat room.");
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Invoked when a connection with the hub is terminated.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is called when a connection with the hub is terminated.
    /// </remarks>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context?.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", string.Empty, $"{username} has left the chat room.");
        await base.OnDisconnectedAsync(exception);
    }
}
