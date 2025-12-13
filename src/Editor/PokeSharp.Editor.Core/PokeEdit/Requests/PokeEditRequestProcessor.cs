using System.Collections.Immutable;
using System.Text.Json;
using Injectio.Attributes;
using PokeSharp.Core;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

[RegisterSingleton]
[AutoServiceShortcut]
public sealed class PokeEditRequestProcessor(IEnumerable<IApiController> controllers, JsonSerializerOptions jsonOptions)
{
    private const int MaxRouteLength = 8;

    private readonly ImmutableDictionary<ApiVerb, ImmutableArray<RouteNode>> _routes = controllers
        .SelectMany(x => x.GetRoutes(jsonOptions))
        .GroupBy(x => x.Verb, tuple => tuple.Node)
        .ToImmutableDictionary(x => x.Key, x => x.OrderBy(y => y.Matcher.Priority).ToImmutableArray());

    public void ProcessRequest(ApiVerb verb, ReadOnlySpan<char> route, Stream requestStream, Stream responseStream)
    {
        Span<RouteParamSlice> paramSlices = stackalloc RouteParamSlice[MaxRouteLength];
        var (buffer, handler) = GetRoute(verb, route, paramSlices);
        handler.Process(buffer, requestStream, responseStream);
    }

    public ValueTask ProcessRequestAsync(
        ApiVerb verb,
        ReadOnlySpan<char> route,
        Stream requestStream,
        Stream responseStream,
        CancellationToken cancellationToken = default
    )
    {
        Span<RouteParamSlice> paramSlices = stackalloc RouteParamSlice[MaxRouteLength];
        var (buffer, handler) = GetRoute(verb, route, paramSlices);
        return handler.ProcessAsync(buffer, requestStream, responseStream, cancellationToken);
    }

    private readonly ref struct HandlerRouteResult(RouteValueBuffer buffer, IRequestHandler handler)
    {
        public RouteValueBuffer Buffer { get; } = buffer;
        public IRequestHandler Handler { get; } = handler;

        public void Deconstruct(out RouteValueBuffer buffer, out IRequestHandler handler)
        {
            buffer = Buffer;
            handler = Handler;
        }
    }

    private HandlerRouteResult GetRoute(ApiVerb verb, ReadOnlySpan<char> route, Span<RouteParamSlice> paramSlices)
    {
        var buffer = new RouteValueBuffer(route, paramSlices);
        var pathSegments = route.Trim('/').Split('/');
        var routes = _routes.GetValueOrDefault(verb, []);
        RouteNode? currentNode = null;
        foreach (var range in pathSegments)
        {
            var segment = route[range];

            if (segment.IsEmpty)
                continue;

            currentNode = null;
            foreach (var node in routes)
            {
                if (!node.Matcher.TryMatch(range.Start.Value, segment, out var slice))
                    continue;

                if (slice is not null)
                {
                    buffer.Add(slice.Value);
                }

                currentNode = node;
                break;
            }

            if (currentNode is null)
                break;

            routes = currentNode.Children;
        }

        var handler = currentNode?.Handler;
        return handler is not null
            ? new HandlerRouteResult(buffer, handler)
            : throw new KeyNotFoundException($"Route {route} not found.");
    }
}
