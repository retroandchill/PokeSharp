namespace PokeSharp.Compiler.Core.Schema;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public abstract class PbsAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class PbsSectionNameAttribute : PbsAttribute;