#pragma once

#include "JsonConverter.h"
#include <type_traits>

namespace PokeEdit
{
    template <typename>
    struct TIsJsonObject : std::false_type
    {
    };

    template <typename T>
    concept TJsonObject = TIsJsonObject<std::remove_cvref_t<T>>::value;

    template <typename>
    struct TJsonObjectTraits;

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

    template <typename>
    struct TIsJsonUnion : std::false_type
    {
    };

    template <typename T>
    concept TJsonUnion = TIsJsonUnion<std::remove_cvref_t<T>>::value;

    template <typename>
    struct TJsonUnionTraits;

    template <typename>
    struct TJsonUnionContainer
    {
        static constexpr bool IsValid = false;
    };

    template <TJsonUnion T>
    struct TJsonUnionContainer<T>
    {
        static constexpr bool IsValid = true;
        using ObjectType = T;

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

    template <TJsonUnion T>
    struct TJsonUnionContainer<TSharedRef<T>>
    {
        static constexpr bool IsValid = true;
        using ObjectType = T;

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
    concept TValidJsonUnionContainer = TJsonUnionContainer<T>::IsValid;

    template <typename>
    struct TIsVariant : std::false_type
    {
    };

    template <typename... T>
    struct TIsVariant<TVariant<T...>> : std::true_type
    {
    };
} // namespace PokeEdit

#define DECLARE_JSON_CONVERTER(Export, Typename)                                                                       \
    template <>                                                                                                        \
    struct PokeEdit::TJsonConverter<Typename>                                                                          \
    {                                                                                                                  \
        Export static std::expected<Typename, FString> Deserialize(const TSharedRef<FJsonValue> &Value);               \
        Export static TSharedRef<FJsonValue> Serialize(const Typename &Value);                                         \
    };

#define DECLARE_JSON_CONVERTERS(Export, Typename)                                                                      \
    DECLARE_JSON_CONVERTER(Export, Typename)                                                                           \
    DECLARE_JSON_CONVERTER(Export, TSharedRef<Typename>)

#define DECLARE_JSON_OBJECT(Export, Typename)                                                                          \
    template <>                                                                                                        \
    struct PokeEdit::TIsJsonObject<Typename> : std::true_type                                                          \
    {                                                                                                                  \
    };                                                                                                                 \
    DECLARE_JSON_CONVERTERS(Export, Typename)

#define DECLARE_JSON_UNION(Export, Typename)                                                                           \
    template <>                                                                                                        \
    struct PokeEdit::TIsJsonUnion<Typename> : std::true_type                                                           \
    {                                                                                                                  \
    };                                                                                                                 \
    DECLARE_JSON_CONVERTERS(Export, Typename)