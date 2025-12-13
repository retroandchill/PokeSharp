using System.Text.Json;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IApiController
{
    IEnumerable<(ApiVerb Verb, RouteNode Node)> GetRoutes(JsonSerializerOptions jsonOptions);
}
