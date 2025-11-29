// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "JsonSerializer.h"

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
    concept TJsonFieldMember = requires
    {
        typename TMemberInfo<Member>::OwnerType;
        typename TMemberInfo<Member>::MemberType;
    };

    template <typename T, auto Member>
    concept TJsonFieldOwner = TJsonFieldMember<Member> && std::same_as<T, typename TMemberInfo<Member>::OwnerType> &&
                              TJsonConvertible<typename TMemberInfo<Member>::MemberType>;

    template <auto Member>
        requires TJsonFieldMember<Member>
    struct TJsonField
    {
        using OwnerType = TMemberInfo<Member>::OwnerType;
        using MemberType = TMemberInfo<Member>::MemberType;

        FStringView Name;
        bool Required;

        constexpr explicit TJsonField(const FStringView InName, const bool InRequired = false)
            : Name(InName), Required(InRequired)
        {
        }

        template <typename T>
            requires std::same_as<std::remove_cvref_t<T>, OwnerType>
        static constexpr decltype(auto) GetMember(T &&Owner)
        {
            return Forward<T>(Owner).*Member;
        }
        
        static constexpr decltype(auto) GetMember(const TSharedRef<OwnerType>& Owner)
        {
            return GetMember(*Owner);
        }

        template <typename T>
            requires std::convertible_to<T, MemberType>
        static constexpr void SetMember(OwnerType &Owner, T &&Value)
        {
            Owner.*Member = Forward<T>(Value);
        }
        
        template <typename T>
            requires std::convertible_to<T, MemberType>
        static constexpr void SetMember(const TSharedRef<OwnerType>& Owner, T &&Value)
        {
            SetMember(*Owner, Forward<T>(Value));
        }

        template <typename... A>
            requires std::constructible_from<MemberType, A...>
        static constexpr void EmplaceMember(OwnerType &Owner, A &&... Args)
        {
            Owner.*Member = MemberType(Forward<A>(Args)...);
        }
        
        template <typename... A>
            requires std::constructible_from<MemberType, A...>
        static constexpr void EmplaceMember(const TSharedRef<OwnerType>& Owner, A &&... Args)
        {
            EmplaceMember(*Owner, Forward<A>(Args)...);
        }
    };

    template <auto Member>
    using TOwnerOf = TMemberInfo<Member>::OwnerType;
    
    template <typename T, auto... Members>
        requires (TJsonFieldMember<Members> && ...)
    struct TJsonObjectType
    {
        using OwnerType = T;
        std::tuple<TJsonField<Members>...> Fields;

        explicit constexpr TJsonObjectType(std::in_place_type_t<T>, TJsonField<Members>... InMembers)
            : Fields(InMembers...)
        {
        }
        
        constexpr TJsonObjectType(std::in_place_type_t<T>, const std::tuple<TJsonField<Members>...>& InMembers)
            : Fields(InMembers)
        {
        }
        
        constexpr TJsonObjectType(std::in_place_type_t<T>, std::tuple<TJsonField<Members>...>&& InMembers)
            : Fields(MoveTemp(InMembers))
        {
        }

        template <typename F>
            requires (std::invocable<F, const TJsonField<Members> &> && ...)
        constexpr void ForEachField(const F &Func) const
        {
            std::apply([&](const auto &... Field) {
                (std::invoke(Func, Field), ...);
            }, Fields);
        }
    };

    template <typename>
    struct TIsJsonObjectType : std::false_type
    {
    };

    template <typename T, auto... Members>
    struct TIsJsonObjectType<TJsonObjectType<T, Members...>> : std::true_type
    {
    };

    template <typename S, typename T>
    concept TValidJsonObjectType =
        TIsJsonObjectType<std::remove_cvref_t<S>>::value && std::same_as<T, typename std::remove_cvref_t<S>::OwnerType>;

    template <typename>
    struct TJsonObjectTraits;

    template <typename T>
    concept TInlineJsonObjectType = requires
    {
        { T::JsonSchema } -> TValidJsonObjectType<T>;
    };

    template <TInlineJsonObjectType T>
    struct TJsonObjectTraits<T>
    {
        static constexpr auto JsonSchema = T::JsonSchema;
    };

    template <typename T>
    concept TJsonObject = requires
    {
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

        static T CreateObject()
        {
            return T();
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

        static TSharedRef<T> CreateObject()
        {
            return MakeShared<T>();
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

    template <typename>
    struct TJsonObjectConverter;

    template <TValidJsonObjectContainer T>
    struct TJsonObjectConverter<T>
    {
        static TValueOrError<T, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            TSharedPtr<FJsonObject> *JsonObject;
            if (!Value->TryGetObject(JsonObject))
            {
                return MakeError(FString::Format(TEXT("Value '{0}' is not an object"), {WriteAsString(Value)}));
            }
            auto Result = TJsonObjectContainer<T>::CreateObject();

            TArray<FString> Errors;
            TJsonObjectContainer<T>::JsonSchema.ForEachField(
                [&Errors, &Result, &JsonObject]<typename F>(const F &Field) {
                    const TSharedPtr<FJsonValue> FieldValue = (*JsonObject)->TryGetField(Field.Name);
                    if (FieldValue == nullptr)
                    {
                        if (Field.Required)
                        {
                            Errors.Add(FString::Format(TEXT("Field '{0}' is required"), {Field.Name}));
                        }

                        return;
                    }

                    TValueOrError<typename F::MemberType, FString> Deserialized =
                        TJsonConverter<typename F::MemberType>::Deserialize(FieldValue.ToSharedRef());

                    if (const auto *Error = Deserialized.TryGetError(); Error != nullptr)
                    {
                        Errors.Add(FString::Format(TEXT("Field '{0}': {1}"), {Field.Name, *Error}));
                    }
                    else
                    {
                        F::SetMember(TJsonObjectContainer<T>::GetMutableObjectRef(Result),
                                     MoveTemp(Deserialized.GetValue()));
                    }
                });

            if (Errors.Num() > 0)
            {
                return MakeError(FString::Join(Errors, TEXT("\n")));
            }

            return MakeValue(Result);
        }

        static TSharedRef<FJsonValue> Serialize(const T &Value)
        {
            auto JsonObject = MakeShared<FJsonObject>();
            TJsonObjectContainer<T>::JsonSchema.ForEachField([&Value, &JsonObject]<typename F>(const F &Field) {
                JsonObject->SetField(FString(Field.Name),
                                     TJsonConverter<typename F::MemberType>::Serialize(F::GetMember(Value)));
            });

            return MakeShared<FJsonValueObject>(JsonObject);
        }
    };

    template <TJsonObject T>
    struct TJsonObjectConverter<TSharedPtr<T>>
    {
        static TValueOrError<TSharedPtr<T>, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            if (Value->IsNull())
            {
                return MakeValue(nullptr);
            }

            return TJsonConverter<TSharedRef<T>>::Deserialize(Value);
        }

        static TSharedRef<FJsonValue> Serialize(const TSharedPtr<T> &Value)
        {
            if (Value == nullptr)
            {
                return MakeShared<FJsonValueNull>();
            }

            return TJsonConverter<TSharedRef<T>>::Serialize(Value.ToSharedRef());
        }
    };

    template <typename T>
    concept HasJsonObjectConverter = TJsonObject<T> && requires(const TSharedRef<FJsonValue> &Value)
                                     {
                                         {
                                             TJsonObjectConverter<T>::Deserialize(Value)
                                         } -> std::same_as<TValueOrError<T, FString>>;
                                     } && requires(const T &Value)
                                     {
                                         {
                                             TJsonObjectConverter<T>::Serialize(Value)
                                         } -> std::same_as<TSharedRef<FJsonValue>>;
                                     };

    template <HasJsonObjectConverter T>
    struct TJsonConverter<T>
    {
        static TValueOrError<T, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            return TJsonObjectConverter<T>::Deserialize(Value);
        }

        static TSharedRef<FJsonValue> Serialize(const T &Value)
        {
            return TJsonObjectConverter<T>::Serialize(Value);
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

        explicit constexpr TJsonUnionKey(const FStringView InKeyName)
            : KeyName(InKeyName)
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
    
    template <typename C, typename V, V (*Ptr)(const C&)>
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
    concept TJsonKeySource = requires
    {
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
        
        constexpr TJsonDiscriminator() : KeyName(TEXT("$type")) { }

        constexpr explicit TJsonDiscriminator(const FStringView InKeyName)
            : KeyName(InKeyName)
        {
        }
    };

    template <auto Ptr>
        requires TJsonKeySource<Ptr>
    constexpr auto JsonDiscriminator = TJsonDiscriminator<Ptr>();
    
    template <typename>
    struct TIsVariant : std::false_type { };
    
    template <typename... T>
    struct TIsVariant<TVariant<T...>> : std::true_type { };
    
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

        constexpr explicit TJsonUnionType(const TJsonDiscriminator<Discriminator> InDiscriminator,
                                          Members... Fields)
            : DiscriminatorMember(InDiscriminator), Fields(Fields...)
        {
        }
        
        constexpr DiscriminatorType GetDiscriminatorValue(const OwnerType& Owner) const
        {
            return std::invoke(Discriminator, Owner);
        }
        
        constexpr DiscriminatorType GetDiscriminatorValue(const TSharedRef<OwnerType>& Owner) const
        {
            return GetDiscriminatorValue(*Owner);
        }
        
        template <typename F, typename T = std::decay_t<std::invoke_result_t<F, const std::tuple_element_t<0, std::tuple<Members...>> &>>>
            requires ((std::invocable<F, const Members &> && std::same_as<std::invoke_result_t<F, const Members &>, T> ) && ...)
        constexpr T ForEachField(const F &Func) const
        {
            T Result;
            std::apply([&](const auto &... Field) {
                (..., (
                    Result.IsSet() ? void() : [&] {
                        if (auto IntermediateResult = std::invoke(Func, Field); IntermediateResult.IsSet())
                        {
                            Result = IntermediateResult.GetValue();
                        }
                    }()
                ));
            }, Fields);
            
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
    concept TInlineJsonUnionType = requires
    {
        { T::JsonSchema } -> TValidJsonObjectType<T>;
    };

    template <TInlineJsonUnionType T>
    struct TJsonUnionTraits<T>
    {
        static constexpr auto JsonSchema = T::JsonSchema;
    };

    template <typename T>
    concept TJsonUnion = requires
    {
        { TJsonUnionTraits<T>::JsonSchema } -> TValidJsonUnionType<T>;
    };
    
    template <typename>
    struct TJsonUnionConverter;
    
    template <typename T>
        requires TJsonUnion<T> && TVariantType<T> && std::same_as<typename std::decay_t<decltype(TJsonUnionTraits<T>::JsonSchema)>::DiscriminatorType, SIZE_T>
    struct TJsonUnionConverter<T>
    {
        static TValueOrError<T, FString> Deserialize(const TSharedRef<FJsonValue>& Value)
        {
            TSharedPtr<FJsonObject> *JsonObject;
            if (!Value->TryGetObject(JsonObject))
            {
                return MakeError(FString::Format(TEXT("Value '{0}' is not an object"), {WriteAsString(Value)}));
            }
            
            auto KeyField = (*JsonObject)->TryGetField(TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName);
            if (KeyField == nullptr)
            {
                return MakeError(FString::Format(TEXT("Field '{0}' is missing from object '{1}'"), {TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName, WriteAsString(Value)}));
            }
            
            auto KeyString = TJsonConverter<FString>::Deserialize(KeyField.ToSharedRef());
            if (const auto *Error = KeyString.TryGetError())
            {
                return MakeError(FString::Format(TEXT("Field '{0}': {1}"), {TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName, *Error}));
            }
            
            auto& Discriminator = KeyString.GetValue();
            TOptional<TValueOrError<T, FString>> Result = TJsonUnionTraits<T>::JsonSchema.ForEachField([&Value, &Discriminator]<typename F>(const F &Field) {
               if (Field.KeyName.Equals(Discriminator, ESearchCase::IgnoreCase))
               {
                   auto Deserialized = TJsonConverter<typename F::ObjectType>::Deserialize(Value);
                   if (auto *Error = Deserialized.TryGetError(); Error != nullptr)
                   {
                       return TOptional<TValueOrError<T, FString>>(MakeError(MoveTemp(*Error)));
                   }
                   
                   return TOptional<TValueOrError<T, FString>>(MakeValue(T(TInPlaceType<typename F::ObjectType>(), MoveTempIfPossible(Deserialized.GetValue()))));
               }
                
                return TOptional<TValueOrError<T, FString>>();
            });
            
            if (Result.IsSet())
            {
                return Result.GetValue();
            }
            
            return MakeError(FString::Format(TEXT("Unknown discriminator value '{0}'"), {Discriminator}));
        }
        
        static TSharedRef<FJsonValue> Serialize(const T &Value)
        {
            auto CurrentDiscriminator = TJsonUnionTraits<T>::JsonSchema.GetDiscriminatorValue(Value);
            TOptional<std::pair<TSharedRef<FJsonValue>, FString>> Result = TJsonUnionTraits<T>::JsonSchema.ForEachField([&CurrentDiscriminator, &Value]<typename F>(const F &Field) 
                {
                    if (Field.DiscriminatorValue == CurrentDiscriminator)
                    {
                        return TOptional<std::pair<TSharedRef<FJsonValue>, FString>>(std::make_pair(TJsonConverter<typename F::ObjectType>::Serialize(Value.template Get<typename F::ObjectType>()), FString(Field.KeyName)));
                    }
                    
                    return TOptional<std::pair<TSharedRef<FJsonValue>, FString>>();
                });
            check(Result.IsSet());
            
            auto [DiscriminatorValue, KeyName] = MoveTemp(Result.GetValue());

            const auto JsonObject = Result->first->AsObject();
            JsonObject->SetField(FString(TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName), MakeShared<FJsonValueString>(MoveTemp(KeyName)));
            
            return DiscriminatorValue;
            
        }
    };
    
    template <typename T>
        requires TJsonUnion<T> && !TVariantType<T>
    struct TJsonUnionConverter<TSharedRef<T>>
    {
        static TValueOrError<TSharedRef<T>, FString> Deserialize(const TSharedRef<FJsonValue>& Value)
        {
            TSharedPtr<FJsonObject> *JsonObject;
            if (!Value->TryGetObject(JsonObject))
            {
                return MakeError(FString::Format(TEXT("Value '{0}' is not an object"), {WriteAsString(Value)}));
            }
            
            auto KeyField = (*JsonObject)->TryGetField(TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName);
            if (KeyField == nullptr)
            {
                return MakeError(FString::Format(TEXT("Field '{0}' is missing from object '{1}'"), {TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName, WriteAsString(Value)}));
            }
            
            auto KeyString = TJsonConverter<FString>::Deserialize(KeyField.ToSharedRef());
            if (const auto *Error = KeyString.TryGetError())
            {
                return MakeError(FString::Format(TEXT("Field '{0}': {1}"), {TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName, *Error}));
            }
            
            auto& Discriminator = KeyString.GetValue();
            TOptional<TValueOrError<TSharedRef<T>, FString>> Result = TJsonUnionTraits<T>::JsonSchema.ForEachField([&Value, &Discriminator]<typename F>(const F &Field) {
               if (Field.KeyName.Equals(Discriminator, ESearchCase::IgnoreCase))
               {
                   auto Deserialized = TJsonConverter<TSharedRef<typename F::ObjectType>>::Deserialize(Value);
                   if (auto *Error = Deserialized.TryGetError(); Error != nullptr)
                   {
                       return TOptional<TValueOrError<TSharedRef<T>, FString>>(MakeError(MoveTemp(*Error)));
                   }
                   
                   return TOptional<TValueOrError<TSharedRef<T>, FString>>(MakeValue(MoveTempIfPossible(Deserialized.GetValue())));
               }
                
                return TOptional<TValueOrError<TSharedRef<T>, FString>>();
            });
            
            if (Result.IsSet())
            {
                return Result.GetValue();
            }
            
            return MakeError(FString::Format(TEXT("Unknown discriminator value '{0}'"), {Discriminator}));
        }
        
        static TSharedRef<FJsonValue> Serialize(const TSharedRef<T>& Value)
        {
            auto CurrentDiscriminator = TJsonUnionTraits<T>::JsonSchema.GetDiscriminatorValue(Value);
            TOptional<std::pair<TSharedRef<FJsonValue>, FString>> Result = TJsonUnionTraits<T>::JsonSchema.ForEachField([&CurrentDiscriminator, &Value]<typename F>(const F &Field) 
                {
                    if (Field.DiscriminatorValue == CurrentDiscriminator)
                    {
                        return TOptional<std::pair<TSharedRef<FJsonValue>, FString>>(std::make_pair(TJsonConverter<TSharedRef<typename F::ObjectType>>::Serialize(StaticCastSharedRef<typename F::ObjectType>(Value)), FString(Field.KeyName)));
                    }
                    
                    return TOptional<std::pair<TSharedRef<FJsonValue>, FString>>();
                });
            check(Result.IsSet());
            
            auto [DiscriminatorValue, KeyName] = MoveTemp(Result.GetValue());

            const auto JsonObject = Result->first->AsObject();
            JsonObject->SetField(FString(TJsonUnionTraits<T>::JsonSchema.DiscriminatorMember.KeyName), MakeShared<FJsonValueString>(MoveTemp(KeyName)));
            
            return DiscriminatorValue;
        }
    };
    

} // namespace PokeEdit