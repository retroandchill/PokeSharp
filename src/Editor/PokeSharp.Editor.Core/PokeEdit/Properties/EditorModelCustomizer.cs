namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditorModelCustomizer
{
    int Priority { get; }

    void OnModelCreating(EditorModelBuilder modelBuilder);
}
