// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/FieldDefinition.h"
#include "Unreachable.h"

namespace PokeEdit
{
    FString LexToString(EFieldKind Kind)
    {
        constexpr std::array KindNames = {TEXT("Text"), TEXT("Int"),        TEXT("Float"),
                                          TEXT("Bool"), TEXT("Choice"),     TEXT("Object"),
                                          TEXT("List"), TEXT("Dictionary"), TEXT("Optional")};
        check(static_cast<uint8>(Kind) < KindNames.size());
        return KindNames[static_cast<uint8>(Kind)];
    }

    bool LexFromString(EFieldKind &OutKind, const FStringView Lex)
    {
        if (auto Result = LexFromString(Lex); Result.IsSet())
        {
            OutKind = Result.GetValue();
            return true;
        }

        return false;
    }

    TOptional<EFieldKind> LexFromString(const FStringView Lex)
    {
        constexpr std::array Literals = {std::make_pair(TEXT("Text"), EFieldKind::Text),
                                         std::make_pair(TEXT("Int"), EFieldKind::Int),
                                         std::make_pair(TEXT("Float"), EFieldKind::Float),
                                         std::make_pair(TEXT("Bool"), EFieldKind::Bool),
                                         std::make_pair(TEXT("Choice"), EFieldKind::Choice),
                                         std::make_pair(TEXT("Object"), EFieldKind::Object),
                                         std::make_pair(TEXT("List"), EFieldKind::List),
                                         std::make_pair(TEXT("Dictionary"), EFieldKind::Dictionary),
                                         std::make_pair(TEXT("Optional"), EFieldKind::Optional)};

        for (const auto &[Literal, Kind] : Literals)
        {
            if (Lex.Equals(Literal, ESearchCase::IgnoreCase))
            {
                return Kind;
            }
        }

        return NullOpt;
    }

    TValueOrError<TSharedRef<FFieldDefinition>, FString> TJsonConverter<TSharedRef<FFieldDefinition>>::Deserialize(
        const TSharedRef<FJsonValue> &Value)
    {
        return TJsonUnionConverter<TSharedRef<FFieldDefinition>>::Deserialize(Value);
    }

    TSharedRef<FJsonValue> TJsonConverter<TSharedRef<FFieldDefinition>>::Serialize(const TSharedRef<FFieldDefinition> &Value)
    {
        return TJsonUnionConverter<TSharedRef<FFieldDefinition>>::Serialize(Value);
    }
    
    template <std::derived_from<FFieldDefinition> T>
        requires (!std::same_as<T, FFieldDefinition>)
    TValueOrError<TSharedRef<T>, FString> TJsonConverter<TSharedRef<T>>::Deserialize(const TSharedRef<FJsonValue> &Value)
    {
        return TJsonObjectConverter<TSharedRef<T>>::Deserialize(Value);
    }

    template <std::derived_from<FFieldDefinition> T>
        requires (!std::same_as<T, FFieldDefinition>)
    TSharedRef<FJsonValue> TJsonConverter<TSharedRef<T>>::Serialize(const TSharedRef<T> &Value)
    {
        return TJsonObjectConverter<TSharedRef<T>>::Serialize(Value);
    }
    
    template struct TJsonConverter<TSharedRef<FBoolFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FTextFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FIntFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FFloatFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FChoiceFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FObjectFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FListFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FDictionaryFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FOptionalFieldDefinition>>;
} // namespace PokeEdit