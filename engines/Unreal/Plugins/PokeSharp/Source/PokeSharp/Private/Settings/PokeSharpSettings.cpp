// Fill out your copyright notice in the Description page of Project Settings.

#include "Settings/PokeSharpSettings.h"
#include "Misc/ConfigCacheIni.h"

UPokeSharpSettings::UPokeSharpSettings()
{
    SetMechanicsGeneration();
}

void UPokeSharpSettings::PostInitProperties()
{
    Super::PostInitProperties();
    SetMechanicsGeneration(false);

    if (const auto *AffectionEffectsProperty =
            GetClass()->FindPropertyByName(GET_MEMBER_NAME_CHECKED(UPokeSharpSettings, AffectionEffects));
        !IsPropertyConfiguredInIni(*AffectionEffectsProperty))
    {
        ApplyHappinessSoftCap = AffectionEffects;
    }
}

#if WITH_EDITOR
void UPokeSharpSettings::PostEditChangeProperty(FPropertyChangedEvent &PropertyChangedEvent)
{
    if (PropertyChangedEvent.GetPropertyName() == GET_MEMBER_NAME_CHECKED(UPokeSharpSettings, MechanicsGeneration))
    {
        SetMechanicsGeneration();
    }
    else if (PropertyChangedEvent.GetPropertyName() == GET_MEMBER_NAME_CHECKED(UPokeSharpSettings, AffectionEffects))
    {
        ApplyHappinessSoftCap = AffectionEffects;
    }

    Super::PostEditChangeProperty(PropertyChangedEvent);
}

FText UPokeSharpSettings::GetSectionText() const
{
    return NSLOCTEXT("PokeSharpSettings", "SectionText", "PokéSharp");
}
#endif

bool UPokeSharpSettings::IsPropertyConfiguredInIni(const FProperty &Property) const
{
    const auto ConfigFilename = GetDefaultConfigFilename();
    const auto SectionNameStr = GetSectionName().ToString();
    const auto KeyName = Property.GetName();

    FString Dummy;
    return GConfig->GetString(*SectionNameStr, *KeyName, Dummy, ConfigFilename);
}

#define SET_IF_NOT_CONFIGURED(Property, ...)                                                                           \
    if (SetAll || !IsPropertyConfiguredInIni(                                                                          \
                      *GetClass()->FindPropertyByName(GET_MEMBER_NAME_CHECKED(UPokeSharpSettings, Property))))         \
    {                                                                                                                  \
        Property = __VA_ARGS__;                                                                                        \
    }

void UPokeSharpSettings::SetMechanicsGeneration(const bool SetAll)
{
    SET_IF_NOT_CONFIGURED(NewBerryPlants, MechanicsGeneration >= 4);
    SET_IF_NOT_CONFIGURED(NewBerryPlants, MechanicsGeneration >= 4);
    SET_IF_NOT_CONFIGURED(PoisonInField, MechanicsGeneration <= 4);
    SET_IF_NOT_CONFIGURED(PoisonFaintInField, MechanicsGeneration <= 3);
    SET_IF_NOT_CONFIGURED(ShinyPokemonChance, MechanicsGeneration >= 6 ? 16 : 8);
    SET_IF_NOT_CONFIGURED(SuperShiny, MechanicsGeneration >= 8);
    SET_IF_NOT_CONFIGURED(LegendariesHaveSomePerfectIVs, MechanicsGeneration >= 6);
    SET_IF_NOT_CONFIGURED(MoveRelearnerCanTeachMoreMoves, MechanicsGeneration >= 6);
    SET_IF_NOT_CONFIGURED(DayCarePokemonGainExpFromWalking, MechanicsGeneration <= 6);
    SET_IF_NOT_CONFIGURED(DayCarePokemonCanShareEggMoves, MechanicsGeneration >= 8);
    SET_IF_NOT_CONFIGURED(BreedingCanInheritMachineMoves, MechanicsGeneration <= 5);
    SET_IF_NOT_CONFIGURED(BreedingCanInheritEggMovesFromMother, MechanicsGeneration >= 6);
    SET_IF_NOT_CONFIGURED(HealStoredPokemon, MechanicsGeneration <= 7);
    SET_IF_NOT_CONFIGURED(RebalancedHealingItemAmounts, MechanicsGeneration >= 7);
    SET_IF_NOT_CONFIGURED(NoVitaminEVCap, MechanicsGeneration >= 8);
    SET_IF_NOT_CONFIGURED(RageCandyBarCuresStatusProblems, MechanicsGeneration >= 7);
    SET_IF_NOT_CONFIGURED(FlutesChangeWildEncounterLevels, MechanicsGeneration >= 6);
    SET_IF_NOT_CONFIGURED(RareCandyUsableAtMaxLevel, MechanicsGeneration >= 8);
    SET_IF_NOT_CONFIGURED(UseMultipleStatItemsAtOnce, MechanicsGeneration >= 8);
    SET_IF_NOT_CONFIGURED(TaughtMachinesKeepOldPP, MechanicsGeneration == 5);
    SET_IF_NOT_CONFIGURED(MoreBonusPremierBalls, MechanicsGeneration >= 8);
    SET_IF_NOT_CONFIGURED(ShowNewSpeciesPokedexEntryMoreOften, MechanicsGeneration >= 7);
    SET_IF_NOT_CONFIGURED(RepelCountsFaintedPokemon, MechanicsGeneration >= 6);
    SET_IF_NOT_CONFIGURED(MoreAbilitiesAffectWildEncounters, MechanicsGeneration >= 8);
    SET_IF_NOT_CONFIGURED(HigherShinyChancesWithNumberBattled, MechanicsGeneration >= 8);
    SET_IF_NOT_CONFIGURED(OverworldWeatherSetsBattleTerrain, MechanicsGeneration >= 8);
    SET_IF_NOT_CONFIGURED(RecalculateTurnOrderAfterMegaEvolution, MechanicsGeneration >= 7);
    SET_IF_NOT_CONFIGURED(RecalculateTurnOrderAfterSpeedChanges, MechanicsGeneration >= 8);
    SET_IF_NOT_CONFIGURED(MoveCategoryPerMove, MechanicsGeneration >= 4);
    SET_IF_NOT_CONFIGURED(NewCriticalHitRateMechanics, MechanicsGeneration >= 6);
    SET_IF_NOT_CONFIGURED(MoreTypeEffects, MechanicsGeneration >= 6);
    SET_IF_NOT_CONFIGURED(BadgesBoosts,
                          {.Attack = MechanicsGeneration >= 4 ? 999 : 1,
                           .Defense = MechanicsGeneration >= 4 ? 999 : 5,
                           .SpecialAttack = MechanicsGeneration >= 4 ? 999 : 7,
                           .SpecialDefense = MechanicsGeneration >= 4 ? 999 : 7,
                           .Speed = MechanicsGeneration >= 4 ? 999 : 3});
    SET_IF_NOT_CONFIGURED(FixedDurationWeatherFromAbility, MechanicsGeneration >= 6);
    SET_IF_NOT_CONFIGURED(XStatItemsRaiseByTwoStages, MechanicsGeneration >= 7);
    SET_IF_NOT_CONFIGURED(NewPokeBallCatchRates, MechanicsGeneration >= 7);
    SET_IF_NOT_CONFIGURED(EnableCriticalCaptures, MechanicsGeneration >= 5);
    SET_IF_NOT_CONFIGURED(NewCaptureCanReplacePartyMember, MechanicsGeneration >= 7);
    SET_IF_NOT_CONFIGURED(ScaledExpFormula, MechanicsGeneration == 5 || MechanicsGeneration >= 7);
    SET_IF_NOT_CONFIGURED(SplitExpBetweenGainers, MechanicsGeneration <= 5);
    SET_IF_NOT_CONFIGURED(MoreExpFromTrainerPokemon, MechanicsGeneration <= 6);
    SET_IF_NOT_CONFIGURED(MoreEVsFromPowerItems, MechanicsGeneration >= 7);
    SET_IF_NOT_CONFIGURED(GainExpForCapture, MechanicsGeneration >= 6);
    SET_IF_NOT_CONFIGURED(CheckEvolutionAfterAllBattles, MechanicsGeneration >= 6);
}

#undef SET_IF_NOT_CONFIGURED