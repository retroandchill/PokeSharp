using System.Runtime.InteropServices;
using Microsoft.Extensions.Primitives;
using PokeSharp.Unreal.Core.Interop;
using UnrealSharp.CoreUObject;
using UnrealSharp.DeveloperSettings;

namespace PokeSharp.Unreal.Core.Configuration;

public class UnrealChangeToken<TUnrealSettings> : IChangeToken
    where TUnrealSettings : UDeveloperSettings
{
    public bool HasChanged { get; private set; }

    public bool ActiveChangeCallbacks => true;

    public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
    {
        return new ChangeTokenCallbackHandler(this, callback, state);
    }

    private sealed unsafe class ChangeTokenCallbackHandler : IDisposable
    {
        private readonly UnrealChangeToken<TUnrealSettings> _owner;
        private readonly Action<object?> _callback;
        private readonly object? _state;
        private FGuid _guid;
        private bool _disposed;

        public ChangeTokenCallbackHandler(
            UnrealChangeToken<TUnrealSettings> owner,
            Action<object?> callback,
            object? state
        )
        {
            _owner = owner;
            _callback = callback;
            _state = state;

            var defaultClass = UObject.GetDefault<TUnrealSettings>().NativeObject;
            var handle = GCHandle.Alloc((Action)Invoke);
            var delegateIntPtr = GCHandle.ToIntPtr(handle);
            var nativeGuid = stackalloc byte[FGuid.NativeDataSize];
            SettingsChangeExporter.CallRegisterSettingsChangeCallback(defaultClass, delegateIntPtr, (IntPtr)nativeGuid);
            _guid = FGuid.FromNative((IntPtr)nativeGuid);
        }

        private void Invoke()
        {
            _callback(_state);
            _owner.HasChanged = true;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            var nativeGuid = stackalloc byte[FGuid.NativeDataSize];
            _guid.ToNative((IntPtr)nativeGuid);
            SettingsChangeExporter.CallUnregisterSettingsChangeCallback((IntPtr)nativeGuid);
            _disposed = true;
        }
    }
}
