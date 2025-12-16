namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IRequestReader
{
    int Read(Span<byte> destination);
}