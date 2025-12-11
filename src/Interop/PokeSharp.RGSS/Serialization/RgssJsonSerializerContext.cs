using System.Text.Json.Serialization;
using PokeSharp.RGSS.RPG;

namespace PokeSharp.RGSS.Serialization;

[RegisterSingleton(Factory = nameof(Default), Duplicate = DuplicateStrategy.Append)]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    AllowOutOfOrderMetadataProperties = true,
    IgnoreReadOnlyProperties = true,
    RespectNullableAnnotations = true,
    AllowTrailingCommas = true
)]
[JsonSerializable(typeof(Dictionary<int, MapInfo>))]
[JsonSerializable(typeof(MapInfo))]
[JsonSerializable(typeof(Map))]
public partial class RgssJsonSerializerContext : JsonSerializerContext;
