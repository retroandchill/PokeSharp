using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IRequestHandler<in TRequest, TResponse>
{
    Name Name { get; }

    TResponse Handle(TRequest request);

    ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
