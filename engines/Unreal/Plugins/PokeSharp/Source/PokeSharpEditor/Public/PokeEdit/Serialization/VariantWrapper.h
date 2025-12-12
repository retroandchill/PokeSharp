// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "InputCoreTypes.h"
#include "Misc/TVariant.h"

template <typename... T>
class TVariantWrapper
{
  public:
    template <typename... A>
        requires std::constructible_from<TVariant<T...>, A...>
    constexpr explicit TVariantWrapper(A &&...Args) : Value(Forward<A>(Args)...)
    {
    }

    template <typename U>
        requires(std::same_as<T, U> || ...)
    constexpr static SIZE_T IndexOfType()
    {
        return TVariant<T...>::template IndexOfType<U>();
    }

    constexpr SIZE_T GetIndex() const
    {
        return Value.GetIndex();
    }

    template <typename U>
        requires(std::same_as<T, U> || ...)
    constexpr bool IsType() const
    {
        return Value.template IsType<U>();
    }

    template <typename U, typename S>
        requires(std::same_as<T, U> || ...)
    constexpr decltype(auto) Get(this S &&Self)
    {
        return std::forward_like<S>(Self.Value.template Get<U>());
    }

    template <typename U>
        requires(std::same_as<T, U> || ...)
    constexpr const U &Get(const TIdentity_T<U> &DefaultValue) const
    {
        return Value.template Get<U>(DefaultValue);
    }

    template <typename U, typename S>
        requires(std::same_as<T, U> || ...)
    constexpr auto *TryGet(this S &Self)
    {
        return Self.Value.template TryGet<U>();
    }

    template <typename U>
        requires(std::same_as<T, U> || ...)
    constexpr void Set(const TIdentity_T<U> &NewValue)
    {
        Value.template Set<U>(NewValue);
    }

    template <typename U>
        requires(std::same_as<T, U> || ...)
    constexpr void Set(TIdentity_T<U>::Type &&NewValue)
    {
        Value.template Set<U>(MoveTemp(NewValue));
    }

    template <typename U, typename... A>
        requires(std::constructible_from<U, A...> && (std::same_as<T, U> || ...))
    constexpr void Emplace(A &&...Args)
    {
        Value.template Emplace<U>(Forward<A>(Args)...);
    }
    
    template <typename F>
        requires (std::invocable<F, T&> || ...)
    constexpr decltype(auto) Visit(F&& Functor)
    {
        return ::Visit(Forward<F>(Functor), Value);
    }
    
    template <typename F>
        requires (std::invocable<F, const T&> || ...)
    constexpr decltype(auto) Visit(F&& Functor) const
    {
        return ::Visit(Forward<F>(Functor), Value);
    }

  private:
    TVariant<T...> Value;
};