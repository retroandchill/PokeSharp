#if UNREAL_ENGINE
using UnrealSharp.Core;
#else
using System.Collections.Concurrent;
#endif
using System.Runtime.CompilerServices;

namespace PokeSharp.Abstractions;

public readonly struct Name : IEquatable<Name>, IEquatable<string>, IEquatable<ReadOnlySpan<char>>, IComparable<Name>
{
#if UNREAL_ENGINE
    private readonly FName _value;
#else
    private readonly uint _comparisonIndex;
    private readonly uint _displayStringIndex;
#endif
    
#if UNREAL_ENGINE
    public Name(FName name)
    {
        _value = name;
    }
#endif
    
    public Name(ReadOnlySpan<char> name)
    {
        #if UNREAL_ENGINE
        _value = new FName(name);
        #else
        _comparisonIndex = NameTable.GetOrAddEntry(name);
        _displayStringIndex = NameTable.GetOrAddEntry(name, true);
#endif
    }

    public Name(string name) : this(name.AsSpan())
    {
        
    }
    
    public static Name None
    {
        get
        {
#if UNREAL_ENGINE
            return new Name(FName.None);
#else
            return new();
#endif
        }
    }

    public bool IsValid
    {
        get
        {
#if UNREAL_ENGINE
            return _value.IsValid;
#else
            return !IsNone;
#endif
        }
    }

    public bool IsNone
    {
        get
        {
#if UNREAL_ENGINE
            return _value.IsNone;
#else
            return this == None;
#endif
        }
    }

    public static bool operator ==(Name lhs, Name rhs)
    {
#if UNREAL_ENGINE
        return lhs._value == rhs._value;
#else
        return lhs._comparisonIndex == rhs._comparisonIndex;
#endif

    }
    
    public static bool operator !=(Name lhs, Name rhs)
    {
        return !(lhs == rhs);
    }
    
    public static bool operator ==(Name lhs, string? rhs)
    {
#if UNREAL_ENGINE
        // TODO: Add additional interop to UnrealSharp to make this comparison directly using StringViews
        return lhs._value == (rhs ?? string.Empty);
#else
        return NameTable.Equals(lhs._comparisonIndex, rhs);
#endif
    }
    
    public static bool operator !=(Name lhs, string? rhs)
    {
        return !(lhs == rhs);
    }


    public static bool operator ==(Name lhs, ReadOnlySpan<char> rhs)
    {
#if UNREAL_ENGINE
        // TODO: Add additional interop to UnrealSharp to make this comparison directly using StringViews
        return lhs._value == rhs.ToString();
#else
        return NameTable.Equals(lhs._comparisonIndex, rhs);
#endif
    }
    
    public static bool operator !=(Name lhs, ReadOnlySpan<char> rhs)
    {
        return !(lhs == rhs);
    }

    public static implicit operator Name(string name) => new(name);
    
    public static implicit operator string(Name name) => name.ToString();
    
#if UNREAL_ENGINE
    public static implicit operator Name(FName name) => new(name);
    
    public static implicit operator FName(Name name) => name._value;
#endif

    public override bool Equals(object? obj)
    {
        return obj is Name other && Equals(other);
    }
    
    public bool Equals(Name other)
    {
#if UNREAL_ENGINE
        return _value == other._value;
#else
        return _comparisonIndex == other._comparisonIndex;
#endif
    }
    public int CompareTo(Name other)
    {
#if UNREAL_ENGINE
        return _value.CompareTo(other._value);
#else
        return (int)(_comparisonIndex - other._comparisonIndex);
#endif
    }

    public bool Equals(string? other)
    {
        return this == other;
    }

    public bool Equals(ReadOnlySpan<char> other)
    {
        return this == other;
    }
    
    public override string ToString()
    {
#if UNREAL_ENGINE
        return _value.ToString();
#else
        return NameTable.GetString(_displayStringIndex);
#endif
    }

    public override int GetHashCode()
    {
#if UNREAL_ENGINE
        return _value.GetHashCode();
#else
        return (int)_comparisonIndex;
#endif
    }
}

#if !UNREAL_ENGINE
internal readonly record struct NameHashEntry(uint Id, int Hash, string Value);

internal static class NameTable
{
    private const int BucketCount = 1024;
    private const int BucketMask = BucketCount - 1;
    
    private static readonly ConcurrentBag<NameHashEntry>[] HashBuckets = new ConcurrentBag<NameHashEntry>[BucketCount];
    private static readonly ConcurrentDictionary<uint, string> IDToString = new();
    
    private static uint _nextId = 1;

    static NameTable()
    {
        for (var i = 0; i < BucketCount; i++)
        {
            HashBuckets[i] = [];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ComputeHash(ReadOnlySpan<char> span, bool caseSensitive = false)
    {
        var hash = 0;
        
        foreach (var t in span)
        {
            var c = caseSensitive ? t : char.ToLowerInvariant(t);
            hash = ((hash << 5) + hash) ^ c;
        }
        return hash;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool SpanEqualsString(ReadOnlySpan<char> span, string str, bool caseSensitive = false)
    {
        if (span.Length != str.Length) return false;
        
        for (var i = 0; i < span.Length; i++)
        {
            if (caseSensitive)
            {
                if (span[i] != str[i])
                    return false;
                
                continue;
            }
            
            if (char.ToLowerInvariant(span[i]) != char.ToLowerInvariant(str[i]))
                return false;
        }
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetOrAddEntry(ReadOnlySpan<char> value, bool caseSensitive = false)
    {
        var hash = ComputeHash(value, caseSensitive);
        var bucketIndex = hash & BucketMask; // Fast modulo for power of 2
        var bucket = HashBuckets[bucketIndex];
        
        // Search existing entries in this bucket
        foreach (var entry in bucket)
        {
            if (entry.Hash == hash && SpanEqualsString(value, entry.Value, caseSensitive))
            {
                return entry.Id;
            }
        }
        
        // Not found, need to add new entry
        // Only now do we allocate a string
        var stringValue = value.ToString();
        var newId = Interlocked.Increment(ref _nextId);
        var newEntry = new NameHashEntry(newId, hash, stringValue);
        
        // Add to both bucket and reverse lookup
        bucket.Add(newEntry);
        IDToString.TryAdd(newId, stringValue);
        
        return newId;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetString(uint id)
    {
        return IDToString.GetValueOrDefault(id, "None");
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(uint id, ReadOnlySpan<char> span)
    {
        if (id == 0)
        {
            return IsNoneSpan(span);
        }
        
        return IDToString.TryGetValue(id, out var value) && SpanEqualsString(span, value);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNoneSpan(ReadOnlySpan<char> span)
    {
        return span.IsEmpty || (span.Length == 4 && 
               (span[0] == 'N' || span[0] == 'n') &&
               (span[1] == 'o' || span[1] == 'O') &&
               (span[2] == 'n' || span[2] == 'N') &&
               (span[3] == 'e' || span[3] == 'E'));
    }
}
#endif