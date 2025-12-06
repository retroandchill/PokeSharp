using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public abstract class EditableMemberBuilder<TSelf>(string name)
    where TSelf : EditableMemberBuilder<TSelf>
{
    internal Name TargetId { get; private set; } = name;
    internal Text TargetDisplayName { get; private set; } = name;

    internal Text TargetTooltip { get; private set; }
    internal Text TargetCategory { get; private set; }

    internal Dictionary<string, string> TargetMetadata { get; } = new();

    public TSelf DisplayName(Text displayName)
    {
        TargetDisplayName = displayName;
        return (TSelf)this;
    }

    public TSelf Tooltip(Text tooltip)
    {
        TargetTooltip = tooltip;
        return (TSelf)this;
    }

    public TSelf Category(Text category)
    {
        TargetCategory = category;
        return (TSelf)this;
    }

    public TSelf Metadata(string key, string? value = null)
    {
        TargetMetadata.Add(key, value ?? string.Empty);
        return (TSelf)this;
    }
}
