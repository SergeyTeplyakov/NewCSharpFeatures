using FluentAssertions;
using Microsoft.VisualBasic.CompilerServices;
using Xunit;
using Xunit.Abstractions;

namespace NewCSharpFeatures._02___CSharp_11._02_Improving_Expressiveness;

/// <summary>
/// Null checking is quite simple but over time, the C# language acquired
/// quite a lot of features that allow checking for objects for null.
/// </summary>
public class NullCheckingSamples(ITestOutputHelper output)
{
    [Fact]
    public void NullChecks()
    {
        // An "old" canonical way for null checking.
        // Not necessarily the best one, because the operator == may
        // be overloaded with a different behavior.
        object? o = null;
        bool isNull = o == null;
        isNull.Should().BeTrue();

        // Ok, but way too verbose.
        isNull = object.ReferenceEquals(o, null);
        isNull.Should().BeTrue();

        // This is kind of a new best way of doing null checks, because
        // the meaning can't be changed by operator overloading.
        // Plus, quite readable!
        isNull = o is null;
        isNull.Should().BeTrue();

        // Using 'not pattern' from C# 10!
        isNull = o is not object o2;
        isNull.Should().BeTrue();

        // Not the best way, for sure!
        isNull = o == default;
        isNull.Should().BeTrue();
    }

    [Fact]
    public void Please_Dont()
    {
        DontDoThisEver? left = new();

        output.WriteLine($"left == null: {left == null}");
        output.WriteLine($"left != null: {left != null}");
        left = null;
        output.WriteLine($"left == null: {left == null}");
        output.WriteLine($"left != null: {left != null}");

        left = new();
        output.WriteLine($"left is null: {left is null}");
        output.WriteLine($"left is not null: {left is not null}");
        
        left = null;
        output.WriteLine($"left is null: {left is null}");
        output.WriteLine($"left is not null: {left is not null}");
    }

    [Fact]
    public void NonNullChecks()
    {
        object? o = null;

        // An "old" canonical way for null checking.
        bool isNotNull = o != null;
        isNotNull.Should().BeFalse();

        // A bit verbose
        isNotNull = !object.ReferenceEquals(o, null);
        isNotNull.Should().BeFalse();

        // Using 'not' pattern from C# 9.
        isNotNull = o is not null;
        isNotNull.Should().BeFalse();

        // An older way with 'is' pattern.
        isNotNull = !(o is null);
        isNotNull.Should().BeFalse();

        // The old 'is' operator can ber used for the null checks.
        isNotNull = o is object;
        isNotNull.Should().BeFalse();

        // Dangerous! o is var x always true!
        isNotNull = o is var x;
        isNotNull.Should().BeTrue();

        isNotNull = o is { } y;
        isNotNull.Should().BeFalse();

        // Can be useful for:
        // if (a.b.c.d is {} d) { ... }
    }

    public class DontDoThisEver
    {
        public static bool operator ==(DontDoThisEver? lhs, DontDoThisEver? rhs) => true; // Everything is null!
        public static bool operator !=(DontDoThisEver? lhs, DontDoThisEver? rhs) => true; // and not null at the same time!
    }
}