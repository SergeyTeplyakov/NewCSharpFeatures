using FluentAssertions;
using Xunit;

namespace NewCSharpFeatures._02___CSharp_11;

/*
 * Records is a tool to model "Value Object" pattern from DDD.
 * A Value Object is an immutable object in software design that represents a specific value with no identity.
 *
 * Bult-in features:
 * -
 * 1. Pattern matching (deconstruction)
 */

public record Point(int X, int Y);

public class PointTests
{
    [Fact]
    public void ShowEquality()
    {
        var p1 = new Point(X: 1, Y: 1);
        var p2 = new Point(X: 1, Y: 1);
        p1.Should().Be(p2); // true. p1 == p2
        p1.Should().NotBeEquivalentTo(p2);
    }
}

// 
// Benefits:
// 1. 
// Show 'with' pattern.
// Show Config pattern where the checks are only happen in construction.
public class Records
{
}