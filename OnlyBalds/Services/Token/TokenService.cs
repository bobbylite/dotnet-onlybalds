using System.Text;
using System.Text.Json;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Options;
using OnlyBalds.Models;

namespace OnlyBalds.Services.Token;

/// <inheritdoc />
public class TokenService : ITokenService
{
    private readonly ReaderWriterLockSlim _tokenLock = new();

    private string _token = String.Empty;

    private readonly ILogger<TokenService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<Auth0ApiOptions> _optionsMonitor;

    /// <inheritdoc />
    public string Token
    {
        get
        {
            _logger.LogDebug("Entering read lock");
            _tokenLock.EnterReadLock();
            try
            {
                _logger.LogDebug("Reading token");
                return _token;
            }
            finally
            {
                _logger.LogDebug("Exiting read lock");
                _tokenLock.ExitReadLock();
            }
        }
        set
        {
            _logger.LogDebug("Entering write lock");
            _tokenLock.EnterWriteLock();
            try
            {
                _logger.LogDebug("Writing token");
                _token = value;
            }
            finally
            {
                _logger.LogDebug("Exiting write lock");
                _tokenLock.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="optionsMonitor">The options monitor.</param>
    /// <exception cref="ArgumentNullException">Thrown if any of the arguments are null.</exception>
    public TokenService(
        ILogger<TokenService> logger,
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<Auth0ApiOptions> optionsMonitor)
    {
        _logger = Guard.Against.Null(logger);
        _httpClientFactory = Guard.Against.Null(httpClientFactory);
        _optionsMonitor = Guard.Against.Null(optionsMonitor);
    }

    /// <inheritdoc />
    /// <exception cref="TokenException">Thrown if an error occurs performing the authentication operation.</exception>
    public async Task AuthenticateAsync()
    {
        _logger.LogInformation("Authenticating with Auth0");
        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.OnlyBaldsAuthenticationToken);

        var data = new
        {
            client_id = _optionsMonitor.CurrentValue.ClientId,
            client_secret = _optionsMonitor.CurrentValue.ClientSecret,
            audience = _optionsMonitor.CurrentValue.Audience,
            grant_type = "client_credentials"
        };

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{_optionsMonitor.CurrentValue.BaseUrl}/oauth/token", content);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get token from Auth0: {StatusCode}", response.StatusCode);
            return;     
        }

        var responseContent = await response.Content.ReadAsStringAsync();

        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        if (tokenResponse is null)
        {
            _logger.LogError("Failed to deserialize token response from Auth0");
            return;
        }
        
        Token = tokenResponse?.AccessToken ?? throw new NullOrEmptyTokenException("An error occurred authenticating with Auth0.");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
