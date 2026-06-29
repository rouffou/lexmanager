using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LexManager.Infrastructure.Endpoints;

public static class EndpointExtensions
{
    /// <summary>Discovers every <see cref="IEndpoint"/> in <paramref name="assembly"/> and registers it.</summary>
    public static IServiceCollection AddEndpointsFrom(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] descriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(descriptors);
        return services;
    }

    /// <summary>Maps all registered endpoints onto the given route group (or the root builder).</summary>
    public static IEndpointRouteBuilder MapRegisteredEndpoints(
        this IEndpointRouteBuilder builder,
        RouteGroupBuilder? routeGroup = null)
    {
        IEnumerable<IEndpoint> endpoints = builder.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>();
        IEndpointRouteBuilder target = routeGroup ?? builder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(target);
        }

        return builder;
    }
}
