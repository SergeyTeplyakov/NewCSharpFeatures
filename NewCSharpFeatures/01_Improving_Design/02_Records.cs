using System.Text.Json.Serialization;

using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace NewCSharpFeatures._02___CSharp_11;

/*
 * Records is a tool to model "Value Object" pattern from DDD.
 * A Value Object is an immutable object in software design that represents a specific value
 * with no identity.
 *
 * Built-in features:
 * - Value equality (like in strings)
 *   Overrides Equals/GetHashCode based on all the fields
 * - Overrides ToString
 * - Supports deconstruction
 * - Supports immutability
 */

#region Basics

public record struct Location2
{
    public int X { get; init; }
    public int Y { get; init; }
    
}

public record Point(int X, int Y);
// https://sharplab.io/#v2:EYLgtghglgdgPgAQEwEYCwAoTCDMACAJwFMBjAewIBM8AFIggZzJgAoEUAGPAMSkYBcAchDBEANHnZcAMhAZCRRAJQBuTEA=

// Using primary constructors from C# 12
public class PointTests(ITestOutputHelper output)
{
    [Fact]
    public void ShowEquality()
    {
        var p1 = new Point(X: 1, Y: 1);
        var p2 = new Point(X: 1, Y: 1);
        
        Assert.True(p1 == p2);
        Assert.True(p1.Equals(p2));
        Assert.True(p1.GetHashCode().Equals(p2.GetHashCode()));
        Assert.False(object.ReferenceEquals(p1, p2));
        
        // Interfaces for Point: IEquatable`1
        output.WriteLine($"Interfaces for {p1.GetType().Name}: {string.Join(", ", p1.GetType().GetInterfaces().Select(i => i.Name))}");
    }

    [Fact]
    public void UsingHashSet()
    {
        var hs = new HashSet<Point>();
        hs.Add(new Point(1, 1));
        Assert.True(hs.Contains(new Point(1, 1)));
        Assert.False(hs.Contains(new Point(1, 2)));
    }

    [Fact]
    public void ShowToString()
    {
        // Prints: Point { X = 1, Y = 1 } 
        output.WriteLine(new Point(1, 1).ToString());
    }

    [Fact]
    public void ShowDeconstruction()
    {
        var p = new Point(1, 2);
        // Can be deconstructed with pattern matching.
        // Only supported with "positional syntax" for property definitions.
        var (x, y) = p;
    }

    [Fact]
    public void ShowNondestructiveMutation()
    {
        var p = new Point(1, 2);
        // Point { X = 1, Y = 2 }
        output.WriteLine(p.ToString());

        p = p with {X = 42};
        // Point { X = 42, Y = 2 }
        output.WriteLine(p.ToString());
    }
}

#endregion Basics

#region Record Structs

// C# 10 feature
public record struct PointStruct(int X, int Y);

public class PointStructTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void RememberAboutDefaultConstructors()
    {
        PointStruct p = default;
        // PointStruct { X = 0, Y = 0 }
        testOutputHelper.WriteLine(p.ToString());

        PointStruct p2 = new PointStruct();
        // PointStruct { X = 0, Y = 0 }
        testOutputHelper.WriteLine(p2.ToString());
    }
}

#endregion Record Structs

#region Using attributes

// You can use '[property:' keyword to specify the target of the attribute.
public record PersonRecord(
    [property: JsonPropertyName("firstName")] string FirstName,
    [property: JsonPropertyName("lastName")] string LastName);
#endregion Using attributes
