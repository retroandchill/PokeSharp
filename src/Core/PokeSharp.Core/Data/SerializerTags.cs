namespace PokeSharp.Core.Data;

/// <summary>
/// Provides constant string tags for serializers supported in the system.
/// </summary>
/// <remarks>
/// This class defines identifiers for different serialization formats, such as JSON
/// and MessagePack. These tags are used across the application to dynamically register
/// and resolve serialization components.
/// </remarks>
public static class SerializerTags
{
    /// <summary>
    /// Represents the name of the MessagePack serialization tag used within the system.
    /// </summary>
    /// <remarks>
    /// This constant is used to identify components and functionalities related to MessagePack
    /// serialization. It is a part of the system's serializer tagging mechanism, allowing for configuration at startup,
    /// to select a specific serialization format.
    /// </remarks>
    public const string MessagePack = "MessagePack";

    /// <summary>
    /// Represents the name of the JSON serialization tag used within the system.
    /// </summary>
    /// <remarks>
    /// This constant is used to identify components and functionalities related to JSON
    /// serialization. It serves as a unique identifier for the JSON format, allowing the
    /// system to dynamically register and resolve serialization-related components.
    /// It is a part of the system's serializer tagging mechanism, allowing for configuration at startup,
    /// to select a specific serialization format.
    /// </remarks>
    public const string Json = "Json";
}
