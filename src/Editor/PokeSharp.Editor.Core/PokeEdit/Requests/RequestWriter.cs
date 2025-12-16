namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IRequestWriter
{
    void Write(ReadOnlySpan<byte> source);

    void Flush();
}