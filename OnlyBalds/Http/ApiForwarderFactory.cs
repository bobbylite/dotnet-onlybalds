using Microsoft.Extensions.ServiceDiscovery.Http;
using OnlyBalds.Services.Token;
using Yarp.ReverseProxy.Forwarder;

namespace OnlyBalds.Http;

/// <summary>
/// Factory for creating instances of <see cref="ForwarderHttpClient"/>.
/// </summary>
internal sealed class ApiForwarderFactory : ForwarderHttpClientFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceDiscoveryHttpMessageHandlerFactory _handlerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiForwarderFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve services.</param>
    /// <param name="handlerFactory">The factory to create instances of <see cref="HttpMessageHandler"/>.</param>
    public ApiForwarderFactory(
        IServiceProvider serviceProvider,
        IServiceDiscoveryHttpMessageHandlerFactory handlerFactory)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
        _handlerFactory = handlerFactory;
    }
    
    /// <summary>
    /// Wraps the provided <see cref="HttpMessageHandler"/> with a handler that adds OAuth token to requests.
    /// </summary>
    /// <param name="context">The context of the HttpClient being created.</param>
    /// <param name="handler">The primary handler to send HTTP requests.</param>
    /// <returns>The wrapped handler.</returns>
    protected override HttpMessageHandler WrapHandler(ForwarderHttpClientContext context, HttpMessageHandler handler)
    {
        var oAuthHandler = _serviceProvider.GetRequiredService<AuthenticationHandler>();
        oAuthHandler.InnerHandler = _handlerFactory.CreateHandler(handler);

        return base.WrapHandler(context, oAuthHandler);
    }
}