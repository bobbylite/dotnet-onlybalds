namespace OnlyBalds.Services.Token;

/// <summary>
/// Represents a service for performing authentication operations with Auth0.
/// </summary>
public interface ITokenService : IDisposable
{
    /// <summary>
    /// The bearer token returned by a previously successful authentication operation.
    /// </summary>
    string Token { get; set; }
    
    /// <summary>
    /// Performs an asynchronous authentication operation with Auth0.
    /// </summary>
    Task AuthenticateAsync();
}
