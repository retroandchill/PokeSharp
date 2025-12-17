using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IPokeEditController
{
    Name Name { get; }
    
    IRequestHandler? GetRequestHandler(Name methodName);
}