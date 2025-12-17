using System.Text.Json.Serialization;
using Injectio.Attributes;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Serialization;

[RegisterSingleton(Factory = nameof(Default), Duplicate = DuplicateStrategy.Append)]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    AllowOutOfOrderMetadataProperties = true,
    IgnoreReadOnlyProperties = true,
    RespectNullableAnnotations = true,
    AllowTrailingCommas = true
)]
[JsonSerializable(typeof(DiffNode))]
[JsonSerializable(typeof(IEnumerable<EditorTabOption>))]
[JsonSerializable(typeof(IEnumerable<Text>))]
[JsonSerializable(typeof(EntityUpdateResponse))]
public partial class PokeEditJsonSerializerContext : JsonSerializerContext;
