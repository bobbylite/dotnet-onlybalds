using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OnlyBalds.Services;
using OnlyBalds.Services.WebState;

namespace OnlyBalds.Hubs;

/// <summary>
/// Represents a SignalR hub for global chat features.
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    
    private readonly ILogger<ChatHub> _logger;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IHuggingFaceInferenceService _huggingFaceInferenceService;
    private readonly IWebStateService _webStateService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatHub"/> class.
    /// </summary>
    /// <param name="hubContext">The context for the hub.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="ChatHub"/> class.
    /// </remarks>
    public ChatHub(
        ILogger<ChatHub> logger,
        IHubContext<ChatHub> hubContext,
        IHuggingFaceInferenceService huggingFaceInferenceService,
        IWebStateService webStateService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(hubContext);
        ArgumentNullException.ThrowIfNull(huggingFaceInferenceService);
        ArgumentNullException.ThrowIfNull(webStateService);

        _logger = logger;
        _hubContext = hubContext;
        _huggingFaceInferenceService = huggingFaceInferenceService;
        _webStateService = webStateService;
    }

    /// <summary>
    /// Sends a message to all clients.
    /// </summary>
    /// <param name="user">The user sending the message.</param>
    /// <param name="message">The message to send.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task SendMessage(string user, string message)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(message);

        var inferences = await _huggingFaceInferenceService.UseRobertaToxicityClassifier(message);

        if (inferences is null)
        {
            _logger.LogError("Failed to retrieve toxicity inference");
            return;
        }

        var toxicityInferences = inferences.FirstOrDefault();

        if (toxicityInferences is null)
        {
            _logger.LogError("Failed to retrieve toxicity inference");
            return;
        }

        foreach(var inference in toxicityInferences)
        {
            if (inference.Label?.ToLower() is not "toxic")
            {
                continue;
            }

            if (inference.Score > .8f)
            {
                await Clients.Caller.SendAsync(
                    "ReceiveMessage",
                    "Moderator", 
                    $"Message from {user} was blocked due to toxicity."
                );

                return;
            }
        }

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
        var emailVerified = Context?.User?.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value;
        var isEmailVerified = !string.IsNullOrEmpty(emailVerified) && bool.Parse(emailVerified);

        if (string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("User connected without a valid identity.");
            return;
        }

        if (isEmailVerified is false)
        {
            _logger.LogWarning("User connected without a verified email.");
            return;
        }

        var users = _webStateService.AddActiveUser(username);
        var userCount = _webStateService.GetActiveUsers();

        _logger.LogDebug("Initializing chat moderation.");
        var inferences = await _huggingFaceInferenceService.UseRobertaToxicityClassifier("Initialize Safe Chat.");

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Moderator", $"{username} has joined.");
        await _hubContext.Clients.All.SendAsync("UserCountChanged", userCount, JsonSerializer.Serialize(users));
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

        if (string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("User connected without a valid identity.");
        }

        var users = _webStateService.RemoveActiveUser(username!);
        var userCount = _webStateService.GetActiveUsers();

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Moderator", $"{username} has left.");
        await _hubContext.Clients.All.SendAsync("UserCountChanged", userCount, JsonSerializer.Serialize(users));
        await base.OnDisconnectedAsync(exception);
    }
}
