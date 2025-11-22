using System.Reflection;

namespace PokeSharp.Core.Saving;

public interface ISaveDataValue
{
    Name Id { get; }

    bool HasNewGameValue { get; }

    bool LoadInBootup { get; }

    bool ResetOnNewGame { get; }

    bool Loaded { get; }

    bool IsValid(object value);

    void Load(object value);

    object Save();

    void LoadNewGameValue();

    void MarkAsUnloaded();
}

public abstract class SaveDataValue<T> : ISaveDataValue
    where T : notnull
{
    public Name Id { get; }

    public bool HasNewGameValue { get; }
    public bool LoadInBootup { get; protected init; }
    public bool ResetOnNewGame { get; protected init; }
    public bool Loaded { get; private set; }

    protected SaveDataValue(Name id)
    {
        Id = id;

        var loadMethod = GetType()
            .GetMethod(nameof(GetNewGameValue), BindingFlags.Instance | BindingFlags.NonPublic, []);
        HasNewGameValue = loadMethod!.DeclaringType != typeof(SaveDataValue<T>);
    }

    public bool IsValid(object value)
    {
        return value is T;
    }

    void ISaveDataValue.Load(object value)
    {
        ValidateValue(value);
        Load((T)value);
        Loaded = true;
    }

    protected abstract void Load(T value);

    object ISaveDataValue.Save() => Save();

    protected abstract T Save();

    void ISaveDataValue.LoadNewGameValue() => Load(GetNewGameValue());

    protected virtual T GetNewGameValue()
    {
        throw new InvalidOperationException($"Save value {Id} has no new game value defined");
    }

    public void MarkAsUnloaded()
    {
        Loaded = false;
    }

    private void ValidateValue(object value)
    {
        if (!IsValid(value))
            throw new ArgumentException($"Save value {Id} is not a {typeof(T).Name} ({value.GetType().Name} given)");
    }
}
