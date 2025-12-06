using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace PokeSharp.Core.Utils;

public static class NullableExtensions
{
    extension<T>([NotNull] T? value)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNull]
        public T RequireNonNull() => value ?? throw new NullReferenceException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNull]
        public T RequireNonNull(string message) => value ?? throw new NullReferenceException(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNull]
        public T RequireNonNull(Func<Exception> exceptionFactory) => value ?? throw exceptionFactory();
    }

    extension<T>([NotNull] T? value)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T RequireNonNull() => value ?? throw new NullReferenceException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T RequireNonNull(string message) => value ?? throw new NullReferenceException(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T RequireNonNull(Func<Exception> exceptionFactory) => value ?? throw exceptionFactory();
    }
}
