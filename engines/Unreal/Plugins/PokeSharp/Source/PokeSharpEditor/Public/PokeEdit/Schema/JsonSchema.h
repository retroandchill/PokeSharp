// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Dom/JsonObject.h"
#include "JsonSerializer.h"
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

        /**
         * The string representation of the name.
         */
        FStringView Name;

        constexpr explicit TJsonField(const FStringView InName) : Name(InName)
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

    template <typename>
    struct TIsMembersTuple : std::false_type
    {
    };

    template <auto... Members>
        requires(TJsonFieldMember<Members> && ...)
    struct TIsMembersTuple<std::tuple<TJsonField<Members>...>> : std::true_type
    {
    };

    template <typename T>
    concept TMembersTuple = TIsMembersTuple<std::remove_cvref_t<T>>::value;

    /**
     * Represents a serializable JSON object type.
     *
     * @tparam T The target type of serialization. May also represent a TSharedRef-wrapped object.
     * @tparam Required The tuple containing all-of-the required serialization members. If any of these members are
     *                  not found, deserialization fails. These members should correspond directly to members that are
     *                  set via the types constructor, which must take a value convertable to that type in the order
     *                  that is specified.
     * @tparam Optional Tuple containing all-of-the additional parameters that do not need to be set during
     *                  deserialization. Each of these must be publically settable, as they are set on the object after
     *                  it is created.
     */
    template <typename T, TMembersTuple Required = std::tuple<>, TMembersTuple Optional = std::tuple<>>
    struct TJsonObjectType
    {
        using OwnerType = T;

        Required RequiredFields;
        Optional OptionalFields;

        constexpr TJsonObjectType(std::in_place_type_t<T>, const Required &InRequired, const Optional &InOptional = {})
            : RequiredFields(InRequired), OptionalFields(InOptional)
        {
        }

        template <typename F>
        constexpr void ForEachField(const F &Func) const
        {
            std::apply([&](const auto &...Field) { (std::invoke(Func, Field), ...); }, RequiredFields);
            std::apply([&](const auto &...Field) { (std::invoke(Func, Field), ...); }, OptionalFields);
        }

        template <typename F>
        constexpr auto ForEachRequiredField(const F &Func) const
        {
            return std::apply([&](const auto &...Field) { return std::make_tuple(std::invoke(Func, Field)...); },
                              RequiredFields);
        }

        template <typename F>
        constexpr void ForEachOptionalField(const F &Func) const
        {
            std::apply([&](const auto &...Field) { (std::invoke(Func, Field), ...); }, OptionalFields);
        }
    };

    template <typename>
    struct TIsJsonObjectType : std::false_type
    {
    };

    template <typename T, TMembersTuple Required, TMembersTuple Optional>
    struct TIsJsonObjectType<TJsonObjectType<T, Required, Optional>> : std::true_type
    {
    };

    template <typename S, typename T>
    concept TValidJsonObjectType =
        TIsJsonObjectType<std::remove_cvref_t<S>>::value && std::same_as<T, typename std::remove_cvref_t<S>::OwnerType>;

    template <typename>
    struct TJsonObjectTraits;

    template <typename T>
    concept TInlineJsonObjectType = requires {
        { T::JsonSchema } -> TValidJsonObjectType<T>;
    };

    template <TInlineJsonObjectType T>
    struct TJsonObjectTraits<T>
    {
        static constexpr auto JsonSchema = T::JsonSchema;
    };

    template <typename T>
    concept TJsonObject = requires {
        { TJsonObjectTraits<T>::JsonSchema } -> TValidJsonObjectType<T>;
    };

    template <typename>
    struct TJsonObjectContainer
    {
        static constexpr bool IsValid = false;
    };

    template <TJsonObject T>
    struct TJsonObjectContainer<T>
    {
        static constexpr bool IsValid = true;
        using ObjectType = T;
        static constexpr auto JsonSchema = TJsonObjectTraits<T>::JsonSchema;

        template <typename... A>
        static T CreateObject(A &&...Args)
        {
            return T(Forward<A>(Args)...);
        }

        static T &GetMutableObjectRef(T &Obj)
        {
            return Obj;
        }

        static const T &GetObjectRef(const T &Obj)
        {
            return Obj;
        }
    };

    template <TJsonObject T>
    struct TJsonObjectContainer<TSharedRef<T>>
    {
        static constexpr bool IsValid = true;
        using ObjectType = T;
        static constexpr auto JsonSchema = TJsonObjectTraits<T>::JsonSchema;

        template <typename... A>
        static TSharedRef<T> CreateObject(A &&...Args)
        {
            return MakeShared<T>(Forward<A>(Args)...);
        }

        static T &GetMutableObjectRef(const TSharedRef<T> &Obj)
        {
            return Obj.Get();
        }

        static const T &GetObjectRef(const TSharedRef<T> &Obj)
        {
            return Obj.Get();
        }
    };

    template <typename T>
    concept TValidJsonObjectContainer = TJsonObjectContainer<T>::IsValid;

    template <TValidJsonObjectContainer T>
    struct TJsonConverter<T>
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
            auto RequiredMembers = TJsonObjectContainer<T>::JsonSchema.ForEachRequiredField(
                [&Errors, &JsonObject]<typename F>(const F &Field)
                {
                    const TSharedPtr<FJsonValue> FieldValue = (*JsonObject)->TryGetField(Field.Name);
                    if (FieldValue == nullptr)
                    {
                        Errors.Add(FString::Format(TEXT("Field '{0}' is required"), {Field.Name}));
                        return TOptional<typename F::MemberType>();
                    }

                    std::expected<typename F::MemberType, FString> Deserialized =
                        TJsonConverter<typename F::MemberType>::Deserialize(FieldValue.ToSharedRef());

                    if (!Deserialized.has_value())
                    {
                        Errors.Add(FString::Format(TEXT("Field '{0}': {1}"), {Field.Name, *Deserialized.error()}));
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
            TJsonObjectContainer<T>::JsonSchema.ForEachOptionalField(
                [&Errors, &Result, &JsonObject]<typename F>(const F &Field)
                {
                    const TSharedPtr<FJsonValue> FieldValue = (*JsonObject)->TryGetField(Field.Name);
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
                        Errors.Add(FString::Format(TEXT("Field '{0}': {1}"), {Field.Name, *Deserialized.error()}));
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
            TJsonObjectContainer<T>::JsonSchema.ForEachField(
                [&Value, &JsonObject]<typename F>(const F &Field)
                {
                    JsonObject->SetField(FString(Field.Name),
                                         TJsonConverter<typename F::MemberType>::Serialize(F::GetMember(Value)));
                });

            return MakeShared<FJsonValueObject>(JsonObject);
        }
    };

    /**
     * JSON Serializer for handling TSharedPtr instances. This will conceptually be the same as the TSharedRef
     * serialization, except that a null value will not cause a serialization error.
     *
     * @tparam T The type of variable that is pointed to
     */
    template <TJsonObject T>
    struct TJsonConverter<TSharedPtr<T>>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<TSharedPtr<T>, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            if (Value->IsNull())
            {
                return TSharedPtr<T>(nullptr);
            }

            return TJsonConverter<TSharedRef<T>>::Deserialize(Value);
        }

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const TSharedPtr<T> &Value)
        {
            if (Value == nullptr)
            {
                return MakeShared<FJsonValueNull>();
            }

            return TJsonConverter<TSharedRef<T>>::Serialize(Value.ToSharedRef());
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

    template <typename>
    struct TIsVariant : std::false_type
    {
    };

    template <typename... T>
    struct TIsVariant<TVariant<T...>> : std::true_type
    {
    };

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
                    (..., (Result.IsSet() ? void() : [&] {
                         if (auto IntermediateResult = std::invoke(Func, Field); IntermediateResult.IsSet())
                         {
                             Result = IntermediateResult.GetValue();
                         }
                     }()));
                },
                Fields);

            return Result;
        }
    };

    template <typename>
    struct TIsJsonUnionType : std::false_type
    {
    };

    template <auto Discriminator, typename... Members>
    struct TIsJsonUnionType<TJsonUnionType<Discriminator, Members...>> : std::true_type
    {
    };

    template <typename S, typename T>
    concept TValidJsonUnionType =
        TIsJsonUnionType<std::remove_cvref_t<S>>::value && std::same_as<T, typename std::remove_cvref_t<S>::OwnerType>;

    template <typename>
    struct TJsonUnionTraits;

    template <typename T>
    concept TInlineJsonUnionType = requires {
        { T::JsonSchema } -> TValidJsonObjectType<T>;
    };

    template <TInlineJsonUnionType T>
    struct TJsonUnionTraits<T>
    {
        static constexpr auto JsonSchema = T::JsonSchema;
    };

    template <typename T>
    concept TJsonUnion = requires {
        { TJsonUnionTraits<T>::JsonSchema } -> TValidJsonUnionType<T>;
    };

    /**
     * JSON converter for discriminated unions that are represented by TVariant.
     *
     * @tparam T The variant type
     */
    template <typename T>
        requires TJsonUnion<T> && TVariantType<T> &&
                 std::same_as<typename std::decay_t<decltype(TJsonUnionTraits<T>::JsonSchema)>::DiscriminatorType,
                              SIZE_T>
    struct TJsonConverter<T>
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

            auto KeyField = (*JsonObject)->TryGetField(TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName);
            if (KeyField == nullptr)
            {
                return std::unexpected(FString::Format(
                    TEXT("Field '{0}' is missing from object '{1}'"),
                    {TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName, WriteAsString(Value)}));
            }

            return TJsonConverter<FString>::Deserialize(KeyField.ToSharedRef())
                .transform_error(
                    [](const FString &Error)
                    {
                        return FString::Format(TEXT("Field '{0}': {1}"),
                                               {TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName, *Error});
                    })
                .and_then(
                    [&Value](const FString &Discriminator) -> std::expected<T, FString>
                    {
                        // We are going to scan through all the discriminators and find the first once that matches.
                        // Once a set optional is returned, we end up skipping all other calls to the callback.
                        TOptional<std::expected<T, FString>> Result = TJsonUnionTraits<T>::JsonSchema.ForEachField(
                            [&Value, &Discriminator]<typename F>(const F &Field) -> TOptional<std::expected<T, FString>>
                            {
                                if (Field.KeyName.Equals(Discriminator, ESearchCase::IgnoreCase))
                                {
                                    return TJsonConverter<typename F::ObjectType>::Deserialize(Value).transform(
                                        [](typename F::ObjectType &&Deserialized) -> T
                                        { return T(TInPlaceType<typename F::ObjectType>(), MoveTemp(Deserialized)); });
                                }

                                return NullOpt;
                            });

                        if (Result.IsSet())
                        {
                            return Result.GetValue();
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
            auto CurrentDiscriminator = TJsonUnionTraits<T>::JsonSchema.GetDiscriminatorValue(Value);
            TOptional<std::pair<TSharedRef<FJsonValue>, FString>> Result = TJsonUnionTraits<T>::JsonSchema.ForEachField(
                [&CurrentDiscriminator, &Value]<typename F>(const F &Field)
                {
                    if (Field.DiscriminatorValue == CurrentDiscriminator)
                    {
                        return TOptional<std::pair<TSharedRef<FJsonValue>, FString>>(
                            std::make_pair(TJsonConverter<typename F::ObjectType>::Serialize(
                                               Value.template Get<typename F::ObjectType>()),
                                           FString(Field.KeyName)));
                    }

                    return TOptional<std::pair<TSharedRef<FJsonValue>, FString>>();
                });
            check(Result.IsSet());

            auto &[DiscriminatorValue, KeyName] = Result.GetValue();

            const auto JsonObject = Result->first->AsObject();
            JsonObject->SetField(FString(TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName),
                                 MakeShared<FJsonValueString>(MoveTemp(KeyName)));

            return DiscriminatorValue;
        }
    };

    /**
     * JSON converter for discriminated unions that are represented by TSharedRef.
     *
     * @tparam T The variant type
     */
    template <typename T>
        requires TJsonUnion<T> && !TVariantType<T>
                              struct TJsonConverter<TSharedRef<T>>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<TSharedRef<T>, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            TSharedPtr<FJsonObject> *JsonObject;
            if (!Value->TryGetObject(JsonObject))
            {
                return std::unexpected(FString::Format(TEXT("Value '{0}' is not an object"), {WriteAsString(Value)}));
            }

            auto KeyField = (*JsonObject)->TryGetField(TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName);
            if (KeyField == nullptr)
            {
                return std::unexpected(FString::Format(
                    TEXT("Field '{0}' is missing from object '{1}'"),
                    {TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName, WriteAsString(Value)}));
            }

            return TJsonConverter<FString>::Deserialize(KeyField.ToSharedRef())
                .transform_error(
                    [](const FString &Error)
                    {
                        return FString::Format(TEXT("Field '{0}': {1}"),
                                               {TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName, *Error});
                    })
                .and_then(
                    [&Value](const FString &Discriminator)
                    {
                        // We are going to scan through all the discriminators and find the first once that matches.
                        // Once a set optional is returned, we end up skipping all other calls to the callback.
                        TOptional<std::expected<TSharedRef<T>, FString>> Result =
                            TJsonUnionTraits<T>::JsonSchema.ForEachField(
                                [&Value, &Discriminator]<typename F>(
                                    const F &Field) -> TOptional<std::expected<TSharedRef<T>, FString>>
                                {
                                    if (Field.KeyName.Equals(Discriminator, ESearchCase::IgnoreCase))
                                    {
                                        return TJsonConverter<TSharedRef<typename F::ObjectType>>::Deserialize(Value)
                                            .transform([](const TSharedRef<typename F::ObjectType> &Deserialized)
                                                       { return StaticCastSharedRef<T>(Deserialized); });
                                    }

                                    return NullOpt;
                                });

                        if (Result.IsSet())
                        {
                            return Result.GetValue();
                        }

                        return std::expected<TSharedRef<T>, FString>(
                            std::unexpect,
                            FString::Format(TEXT("Unknown discriminator value '{0}'"), {Discriminator}));
                    });
        }

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const TSharedRef<T> &Value)
        {
            auto CurrentDiscriminator = TJsonUnionTraits<T>::JsonSchema.GetDiscriminatorValue(Value);

            // We are going to scan through all the discriminators and find the first once that matches.
            // Once a set optional is returned, we end up skipping all other calls to the callback.
            TOptional<std::pair<TSharedRef<FJsonValue>, FString>> Result = TJsonUnionTraits<T>::JsonSchema.ForEachField(
                [&CurrentDiscriminator, &Value]<typename F>(const F &Field)
                {
                    if (Field.DiscriminatorValue == CurrentDiscriminator)
                    {
                        return TOptional<std::pair<TSharedRef<FJsonValue>, FString>>(
                            std::make_pair(TJsonConverter<TSharedRef<typename F::ObjectType>>::Serialize(
                                               StaticCastSharedRef<typename F::ObjectType>(Value)),
                                           FString(Field.KeyName)));
                    }

                    return TOptional<std::pair<TSharedRef<FJsonValue>, FString>>();
                });
            check(Result.IsSet());

            auto [DiscriminatorValue, KeyName] = MoveTemp(Result.GetValue());

            const auto JsonObject = Result->first->AsObject();
            JsonObject->SetField(FString(TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName),
                                 MakeShared<FJsonValueString>(MoveTemp(KeyName)));

            return DiscriminatorValue;
        }
    };

} // namespace PokeEdit