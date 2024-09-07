using Microsoft.Extensions.ServiceDiscovery.Http;
using Yarp.ReverseProxy.Forwarder;

namespace OnlyBalds.Http;

/// <summary>
/// Represents a factory for creating instances of <see cref="ForwarderHttpClient"/>.
/// </summary>
/// <remarks>
/// This class is used to create instances of <see cref="ForwarderHttpClient"/>.
/// </remarks>
/// <seealso cref="ForwarderHttpClientFactory" />
/// <seealso cref="IForwarderHttpClientFactory" />
internal sealed class ApiForwarderFactory : ForwarderHttpClientFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceDiscoveryHttpMessageHandlerFactory _handlerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiForwarderFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="handlerFactory">The handler factory.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="ApiForwarderFactory"/> class.
    /// </remarks>
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
    /// Wraps the handler with the specified context.
    /// </summary>
    /// <param name="context">The forwarder HTTP client context.</param>
    /// <param name="handler">The HTTP message handler.</param>
    /// <returns>The wrapped HTTP message handler.</returns>
    /// <remarks>
    /// This method wraps the handler with the specified context.
    /// </remarks>
    /// <seealso cref="ForwarderHttpClientFactory.WrapHandler" />
    protected override HttpMessageHandler WrapHandler(ForwarderHttpClientContext context, HttpMessageHandler handler)
    {
        var oAuthHandler = _serviceProvider.GetRequiredService<AuthenticationHandler>();
        oAuthHandler.InnerHandler = _handlerFactory.CreateHandler(handler);

        return base.WrapHandler(context, oAuthHandler);
    }
}