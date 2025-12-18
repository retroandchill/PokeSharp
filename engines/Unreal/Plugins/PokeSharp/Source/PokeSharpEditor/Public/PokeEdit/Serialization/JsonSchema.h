// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Dom/JsonObject.h"
#include "JsonConverterTemplates.h"
#include "JsonHelpers.h"
#include "JsonSchemaFwd.h"
#include <bit>

namespace PokeEdit
{
    template <auto MemberPtr>
    struct TMemberInfo;

    template <typename C, typename M, M C::*Ptr>
    struct TMemberInfo<Ptr>
    {
        using OwnerType = C;
        using MemberType = M;
    };

    template <auto Member>
    concept TJsonFieldMember = requires {
        typename TMemberInfo<Member>::OwnerType;
        typename TMemberInfo<Member>::MemberType;
    };

    template <typename T, auto Member>
    concept TJsonFieldOwner = TJsonFieldMember<Member> && std::same_as<T, typename TMemberInfo<Member>::OwnerType> &&
                              TJsonConvertible<typename TMemberInfo<Member>::MemberType>;

    /**
     * Represents a fields in the serialized JSON representation.
     *
     * @tparam Member The member pointer used to retrieve and set the value.
     * @note This type stores an FStringView to the name, as this type is supposed to be instantiated in a constexpr
     *       context and point to a compile-time literal.
     */
    template <auto Member>
        requires TJsonFieldMember<Member>
    struct TJsonField
    {
        using OwnerType = TMemberInfo<Member>::OwnerType;
        using MemberType = TMemberInfo<Member>::MemberType;

        FStringView CppName;
        FStringView JsonName;
        bool Required;

        constexpr explicit TJsonField(const FStringView InCppName, const FStringView InJsonName, const bool InRequired)
            : CppName(InCppName), JsonName(InJsonName), Required(InRequired)
        {
        }

        template <auto Str>
        constexpr explicit TJsonField(const FStringView InCppName,
                                      TStaticStringView<Str> StaticString,
                                      const bool InRequired)
            : CppName(InCppName), JsonName(StaticString.Value), Required(InRequired)
        {
        }

        template <typename T>
            requires std::same_as<std::remove_cvref_t<T>, OwnerType>
        static constexpr decltype(auto) GetMember(T &&Owner)
        {
            return Forward<T>(Owner).*Member;
        }

        static constexpr decltype(auto) GetMember(const TSharedRef<OwnerType> &Owner)
        {
            return GetMember(*Owner);
        }

        template <typename T>
            requires std::convertible_to<T, MemberType>
        static constexpr void SetMember(OwnerType &Owner, T &&Value)
        {
            const auto BaseAddr = reinterpret_cast<std::uintptr_t>(&Owner);
            const auto FieldAddr = reinterpret_cast<std::uintptr_t>(&(Owner.*Member));
            const auto Offset = FieldAddr - BaseAddr;
            UE_LOG(LogTemp, Warning, TEXT("Offset is %llu"), Offset)
            Owner.*Member = Forward<T>(Value);
        }

        template <typename... A>
            requires std::constructible_from<MemberType, A...>
        static constexpr void EmplaceMember(OwnerType &Owner, A &&...Args)
        {
            Owner.*Member = MemberType(Forward<A>(Args)...);
        }
    };

    template <auto Member>
    using TOwnerOf = TMemberInfo<Member>::OwnerType;

    /**
     * Represents a serializable JSON object type.
     *
     * @tparam T The target type of serialization. May also represent a TSharedRef-wrapped object.
     * @tparam Members The tuple containing all-of-the required serialization members. If any of these members are
     *                  not found, deserialization fails. These members should correspond directly to members that are
     *                  set via the types constructor, which must take a value convertable to that type in the order
     *                  that is specified.
     */
    template <typename T, auto... Members>
    struct TJsonObjectType
    {
        using OwnerType = T;
        std::tuple<TJsonField<Members>...> Fields;

        constexpr explicit TJsonObjectType(std::in_place_type_t<T>, TJsonField<Members>... InFields)
            : Fields(InFields...)
        {
        }

        template <typename F>
        constexpr void ForEachField(const F &Func) const
        {
            std::apply([&](const auto &...Field) { (std::invoke(Func, Field), ...); }, Fields);
        }

        template <typename F>
        constexpr void ForEachFieldWithBreak(const F &Func) const
        {
            std::apply([&](const auto &...Field) { (std::invoke(Func, Field) && ...); }, Fields);
        }
    };

    template <typename>
    struct TMemberVariantType;

    template <typename T, auto... Members>
        requires(sizeof...(Members) > 0)
    struct TMemberVariantType<TJsonObjectType<T, Members...>>
    {
        using FType = TVariant<TJsonField<Members>...>;
    };

    template <typename T, auto... Members>
        requires(sizeof...(Members) == 0)
    struct TMemberVariantType<TJsonObjectType<T, Members...>>
    {
        using FType = TVariant<std::monostate>;
    };

    template <typename T>
    concept TValidMemberVariant = requires { typename TMemberVariantType<std::remove_cvref_t<T>>::FType; };

    template <TValidMemberVariant T>
    using TMemberVariant = TMemberVariantType<std::remove_cvref_t<T>>::FType;

    template <std::invocable T>
    consteval auto GetRequiredMembers(T Factory)
    {
        return FilterTuple([](const auto &Member) { return Member.Required; }, Factory);
    }

    template <std::invocable T>
    consteval auto GetOptionalMembers(T Factory)
    {
        return FilterTuple([](const auto &Member) { return !Member.Required; }, Factory);
    }

    template <typename T, typename F>
    constexpr auto ForEachRequiredField(T Factory, F Func)
    {
        return std::apply([&](const auto &...Field) { return std::make_tuple(std::invoke(Func, Field)...); },
                          GetRequiredMembers(Factory));
    }

    template <typename T, typename F>
    constexpr void ForEachOptionalField(T Factory, F Func)
    {
        std::apply([&](const auto &...Field) { (std::invoke(Func, Field), ...); }, GetOptionalMembers(Factory));
    }

    template <TValidJsonObjectContainer T>
    constexpr auto TJsonObjectSchema = TJsonObjectTraits<typename TJsonObjectContainer<T>::ObjectType>::JsonSchema;

    template <typename>
    struct TJsonObjectConverter;

    template <TValidJsonObjectContainer T>
    struct TJsonObjectConverter<T>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<T, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            TSharedPtr<FJsonObject> *JsonObject;
            if (!Value->TryGetObject(JsonObject))
            {
                return std::unexpected(FString::Format(TEXT("Value '{0}' is not an object"), {WriteAsString(Value)}));
            }
            TArray<FString> Errors;

            // We first need to gather a tuple of all the required members, and so long as they are all set (which would
            // also mean no-errors), we can then apply that transformation to construct the object.
            auto RequiredMembers = ForEachRequiredField(
                [] { return TJsonObjectSchema<T>.Fields; },
                [&Errors, &JsonObject]<typename F>(const F &Field)
                {
                    const TSharedPtr<FJsonValue> FieldValue = (*JsonObject)->TryGetField(Field.JsonName);
                    if (FieldValue == nullptr)
                    {
                        Errors.Add(FString::Format(TEXT("Field '{0}' is required"), {Field.JsonName}));
                        return TOptional<typename F::MemberType>();
                    }

                    std::expected<typename F::MemberType, FString> Deserialized =
                        TJsonConverter<typename F::MemberType>::Deserialize(FieldValue.ToSharedRef());

                    if (!Deserialized.has_value())
                    {
                        Errors.Add(FString::Format(TEXT("Field '{0}': {1}"), {Field.JsonName, *Deserialized.error()}));
                        return TOptional<typename F::MemberType>();
                    }

                    return TOptional<typename F::MemberType>(MoveTempIfPossible(Deserialized).value());
                });

            if (Errors.Num() > 0)
            {
                return std::unexpected(FString::Join(Errors, TEXT("\n")));
            }

            auto Result =
                std::apply([](const auto &...Field)
                           { return TJsonObjectContainer<T>::CreateObject(MoveTempIfPossible(Field.GetValue())...); },
                           RequiredMembers);

            // Now that we constructed the object from the required fields, loop through all the others and set those
            // If any get found.
            ForEachOptionalField(
                [] { return TJsonObjectSchema<T>.Fields; },
                [&Errors, &Result, &JsonObject]<typename F>(const F &Field)
                {
                    const TSharedPtr<FJsonValue> FieldValue = (*JsonObject)->TryGetField(Field.JsonName);
                    if (FieldValue == nullptr)
                        return;

                    std::expected<typename F::MemberType, FString> Deserialized =
                        TJsonConverter<typename F::MemberType>::Deserialize(FieldValue.ToSharedRef());
                    if (Deserialized.has_value())
                    {
                        F::SetMember(TJsonObjectContainer<T>::GetMutableObjectRef(Result),
                                     MoveTemp(Deserialized).value());
                    }
                    else
                    {
                        Errors.Add(FString::Format(TEXT("Field '{0}': {1}"), {Field.JsonName, *Deserialized.error()}));
                    }
                });

            if (Errors.Num() > 0)
            {
                return std::unexpected(FString::Join(Errors, TEXT("\n")));
            }

            return MoveTempIfPossible(Result);
        }

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const T &Value)
        {
            auto JsonObject = MakeShared<FJsonObject>();
            TJsonObjectSchema<T>.ForEachField(
                [&Value, &JsonObject]<typename F>(const F &Field)
                {
                    JsonObject->SetField(FString(Field.JsonName),
                                         TJsonConverter<typename F::MemberType>::Serialize(F::GetMember(Value)));
                });

            return MakeShared<FJsonValueObject>(JsonObject);
        }
    };

    template <TJsonObject T, auto V>
        requires std::equality_comparable_with<decltype(V), decltype(V)>
    struct TJsonUnionKey
    {
        using ObjectType = T;
        using DiscriminatorType = decltype(V);

        FStringView KeyName;
        static constexpr DiscriminatorType DiscriminatorValue = V;

        explicit constexpr TJsonUnionKey(const FStringView InKeyName) : KeyName(InKeyName)
        {
        }
    };

    template <typename>
    struct TIsJsonUnionKey : std::false_type
    {
    };

    template <TJsonObject T, auto V>
        requires std::equality_comparable_with<decltype(V), decltype(V)>
    struct TIsJsonUnionKey<TJsonUnionKey<T, V>> : std::true_type
    {
    };

    template <typename T>
    concept TValidJsonUnionKey = TIsJsonUnionKey<std::remove_cvref_t<T>>::value;

    template <auto>
    struct TJsonKeySourceTraits;

    template <typename C, typename V, V (C::*Ptr)() const>
    struct TJsonKeySourceTraits<Ptr>
    {
        using OwnerType = C;
        using MemberType = std::decay_t<V>;
    };

    template <typename C, typename V, V (*Ptr)(const C &)>
    struct TJsonKeySourceTraits<Ptr>
    {
        using OwnerType = C;
        using MemberType = std::decay_t<V>;
    };

    template <typename C, typename V, V C::*Ptr>
    struct TJsonKeySourceTraits<Ptr>
    {
        using OwnerType = C;
        using MemberType = std::decay_t<V>;
    };

    template <auto Ptr>
    concept TJsonKeySource = requires {
        typename TJsonKeySourceTraits<Ptr>::OwnerType;
        typename TJsonKeySourceTraits<Ptr>::MemberType;
    };

    template <auto Ptr>
        requires TJsonKeySource<Ptr>
    struct TJsonDiscriminator
    {
        using ObjectType = TJsonKeySourceTraits<Ptr>::OwnerType;
        using DiscriminatorType = TJsonKeySourceTraits<Ptr>::MemberType;

        FStringView KeyName;

        constexpr TJsonDiscriminator() : KeyName(TEXT("$type"))
        {
        }

        constexpr explicit TJsonDiscriminator(const FStringView InKeyName) : KeyName(InKeyName)
        {
        }
    };

    template <auto Ptr>
        requires TJsonKeySource<Ptr>
    constexpr auto JsonDiscriminator = TJsonDiscriminator<Ptr>();

    template <typename T>
    concept TVariantType = TIsVariant<T>::value;

    template <auto Discriminator, typename... Members>
        requires TJsonKeySource<Discriminator> &&
                 ((TValidJsonUnionKey<Members> &&
                   std::same_as<typename Members::DiscriminatorType,
                                typename TJsonKeySourceTraits<Discriminator>::MemberType>) &&
                  ...)
    struct TJsonUnionType
    {
        using OwnerType = TJsonKeySourceTraits<Discriminator>::OwnerType;
        using DiscriminatorType = TJsonKeySourceTraits<Discriminator>::MemberType;

        TJsonDiscriminator<Discriminator> DiscriminatorMember;
        std::tuple<Members...> Fields;

        constexpr explicit TJsonUnionType(const TJsonDiscriminator<Discriminator> InDiscriminator, Members... Fields)
            : DiscriminatorMember(InDiscriminator), Fields(Fields...)
        {
        }

        constexpr DiscriminatorType GetDiscriminatorValue(const OwnerType &Owner) const
        {
            return std::invoke(Discriminator, Owner);
        }

        constexpr DiscriminatorType GetDiscriminatorValue(const TSharedRef<OwnerType> &Owner) const
        {
            return GetDiscriminatorValue(*Owner);
        }

        template <
            typename F,
            typename T = std::decay_t<std::invoke_result_t<F, const std::tuple_element_t<0, std::tuple<Members...>> &>>>
            requires((std::invocable<F, const Members &> &&
                      std::same_as<std::invoke_result_t<F, const Members &>, T>) &&
                     ...)
        constexpr T ForEachField(const F &Func) const
        {
            // A precondition of this method is that T represents TOptional of some value. What we will do is call
            // invoke, and if we get a value, set that into result. Once the value gets set, all other
            // operations become a no-op. Ideally this would best be refactored to use a template for, once
            // C++ 26 is supported.
            T Result;
            std::apply(
                [&](const auto &...Field)
                {
                    (void)([&] {
                         if (auto IntermediateResult = std::invoke(Func, Field); IntermediateResult.IsSet())
                         {
                             Result = IntermediateResult.GetValue();
                             return false;
                         }
                        
                        return true;
                     }() && ...);
                },
                Fields);

            return Result;
        }
    };

    template <TValidJsonUnionContainer T>
    constexpr auto TJsonUnionSchema = TJsonUnionTraits<typename TJsonUnionContainer<T>::ObjectType>::JsonSchema;

    /**
     * JSON converter for discriminated unions that are represented by TVariant.
     *
     * @tparam T The variant type
     */
    template <TValidJsonUnionContainer T>
    struct TJsonObjectConverter<T>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<T, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            TSharedPtr<FJsonObject> *JsonObject;
            if (!Value->TryGetObject(JsonObject))
            {
                return std::unexpected(FString::Format(TEXT("Value '{0}' is not an object"), {WriteAsString(Value)}));
            }

            auto KeyField = (*JsonObject)->TryGetField(TJsonUnionSchema<T>.DiscriminatorMember.KeyName);
            if (KeyField == nullptr)
            {
                return std::unexpected(
                    FString::Format(TEXT("Field '{0}' is missing from object '{1}'"),
                                    {TJsonUnionSchema<T>.DiscriminatorMember.KeyName, WriteAsString(Value)}));
            }

            return TJsonConverter<FString>::Deserialize(KeyField.ToSharedRef())
                .transform_error(
                    [](const FString &Error)
                    {
                        return FString::Format(TEXT("Field '{0}': {1}"),
                                               {TJsonUnionSchema<T>.DiscriminatorMember.KeyName, *Error});
                    })
                .and_then(
                    [&Value](const FString &Discriminator) -> std::expected<T, FString>
                    {
                        // We are going to scan through all the discriminators and find the first once that matches.
                        // Once a set optional is returned, we end up skipping all other calls to the callback.
                        TOptional<std::expected<T, FString>> Result = TJsonUnionSchema<T>.ForEachField(
                            [&Value, &Discriminator]<typename F>(const F &Field) -> TOptional<std::expected<T, FString>>
                            {
                                if (Field.KeyName.Equals(Discriminator, ESearchCase::IgnoreCase))
                                {
                                    return TJsonConverter<typename F::ObjectType>::Deserialize(Value).transform(
                                        [](typename F::ObjectType &&Deserialized) -> T
                                        {
                                            return TJsonUnionContainer<T>::CreateObject(
                                                TInPlaceType<typename F::ObjectType>(),
                                                MoveTemp(Deserialized));
                                        });
                                }

                                return NullOpt;
                            });

                        if (Result.IsSet())
                        {
                            return MoveTemp(Result.GetValue());
                        }

                        return std::unexpected(
                            FString::Format(TEXT("Unknown discriminator value '{0}'"), {Discriminator}));
                    });
        }

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const T &Value)
        {
            // We are going to scan through all the discriminators and find the first once that matches.
            // Once a set optional is returned, we end up skipping all other calls to the callback.
            auto &ValueReference = TJsonUnionContainer<T>::GetObjectRef(Value);

            auto CurrentDiscriminator = TJsonUnionSchema<T>.GetDiscriminatorValue(ValueReference);
            TOptional<std::pair<TSharedRef<FJsonValue>, FString>> Result = TJsonUnionSchema<T>.ForEachField(
                [&CurrentDiscriminator, &ValueReference]<typename F>(const F &Field)
                {
                    if (Field.DiscriminatorValue == CurrentDiscriminator)
                    {
                        return TOptional<std::pair<TSharedRef<FJsonValue>, FString>>(
                            std::make_pair(TJsonConverter<typename F::ObjectType>::Serialize(
                                               ValueReference.template Get<typename F::ObjectType>()),
                                           FString(Field.KeyName)));
                    }

                    return TOptional<std::pair<TSharedRef<FJsonValue>, FString>>();
                });
            check(Result.IsSet());

            auto &[DiscriminatorValue, KeyName] = Result.GetValue();

            const auto JsonObject = Result->first->AsObject();
            JsonObject->SetField(FString(TJsonUnionSchema<T>.DiscriminatorMember.KeyName),
                                 MakeShared<FJsonValueString>(MoveTemp(KeyName)));

            return DiscriminatorValue;
        }
    };

} // namespace PokeEdit

#define DEFINE_JSON_CONVERTER(Typename)                                                                                \
    std::expected<Typename, FString> PokeEdit::TJsonConverter<Typename>::Deserialize(                                  \
        const TSharedRef<FJsonValue> &Value)                                                                           \
    {                                                                                                                  \
        return TJsonObjectConverter<Typename>::Deserialize(Value);                                                     \
    }                                                                                                                  \
    TSharedRef<FJsonValue> PokeEdit::TJsonConverter<Typename>::Serialize(const Typename &Value)                        \
    {                                                                                                                  \
        return TJsonObjectConverter<Typename>::Serialize(Value);                                                       \
    }

#define DEFINE_JSON_CONVERTERS(Typename)                                                                               \
    DEFINE_JSON_CONVERTER(Typename)                                                                                    \
    DEFINE_JSON_CONVERTER(TSharedRef<Typename>)

#define JSON_OBJECT_SCHEMA_BEGIN(TypeName)                                                                             \
    template <>                                                                                                        \
    struct PokeEdit::TJsonObjectTraits<TypeName>                                                                       \
    {                                                                                                                  \
        using Type = TypeName;                                                                                         \
        static constexpr auto JsonSchema = PokeEdit::TJsonObjectType( \
            std::in_place_type<TypeName>

#define JSON_OBJECT_SCHEMA_END                                                                                         \
        );                                                                                                             \
    }                                                                                                                  \
    ;

#define JSON_FIELD_INTERNAL(FieldName, Name, Required)                                                                 \
    , PokeEdit::TJsonField<&Type::FieldName>(TEXT(#FieldName), Name, Required)

#define JSON_FIELD_NAMED_OPTIONAL(FieldName, Name) JSON_FIELD_INTERNAL(FieldName, Name, false)

#define JSON_FIELD_NAMED_REQUIRED(FieldName, Name) JSON_FIELD_INTERNAL(FieldName, Name, true)

#define JSON_FIELD_OPTIONAL(FieldName)                                                                                 \
    JSON_FIELD_NAMED_OPTIONAL(FieldName, TStaticStringView<PokeEdit::ToCamelCase(TEXT(#FieldName))>{})

#define JSON_FIELD_REQUIRED(FieldName)                                                                                 \
    JSON_FIELD_NAMED_REQUIRED(FieldName, TStaticStringView<PokeEdit::ToCamelCase(TEXT(#FieldName))>{})

#define JSON_VARIANT_BEGIN(TypeName)                                                                                   \
    template <>                                                                                                        \
    struct PokeEdit::TJsonUnionTraits<TypeName>                                                                        \
    {                                                                                                                  \
        using Type = TypeName;                                                                                         \
        static constexpr auto JsonSchema = PokeEdit::TJsonUnionType( \
            PokeEdit::TJsonDiscriminator<&TypeName::GetIndex>()

#define JSON_VARIANT_END );                                                                                            \
    }                                                                                                                  \
    ;

#define JSON_VARIANT_TYPE(TypeName, Discriminator)                                                                     \
    , PokeEdit::TJsonUnionKey<TypeName, Type::IndexOfType<TypeName>()>(Discriminator)