using System.Text.Json.Serialization;
using Injectio.Attributes;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Serialization.Json;

[RegisterSingleton(Factory = nameof(Default), Duplicate = DuplicateStrategy.Append)]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    AllowOutOfOrderMetadataProperties = true,
    IgnoreReadOnlyProperties = true,
    RespectNullableAnnotations = true,
    AllowTrailingCommas = true
)]
[JsonSerializable(typeof(PokemonType))]
[JsonSerializable(typeof(Ability))]
[JsonSerializable(typeof(Move))]
[JsonSerializable(typeof(Item))]
[JsonSerializable(typeof(BerryPlant))]
[JsonSerializable(typeof(Species))]
[JsonSerializable(typeof(SpeciesMetrics))]
[JsonSerializable(typeof(ShadowPokemon))]
[JsonSerializable(typeof(Metadata))]
[JsonSerializable(typeof(PlayerMetadata))]
[JsonSerializable(typeof(Ribbon))]
[JsonSerializable(typeof(TrainerType))]
[JsonSerializable(typeof(EnemyTrainer))]
[JsonSerializable(typeof(TownMap))]
[JsonSerializable(typeof(Encounter))]
[JsonSerializable(typeof(MapConnection))]
[JsonSerializable(typeof(RegionalDex))]
public partial class PokeSharpJsonContext : JsonSerializerContext;
