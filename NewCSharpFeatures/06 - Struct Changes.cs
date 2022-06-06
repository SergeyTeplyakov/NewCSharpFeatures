using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace NewCSharpFeatures._02___CSharp_11;

public class StructTests
{
    [Fact]
    public void DefaultDoesNotCallConstructor()
    {
        TaskSourceSlim<object> tss = default;
        ((object) tss._tcs!).Should().BeNull();
    }

    #region TaskSourceSlim
    // C# 10 supports default constructors in structs!
    public readonly struct TaskSourceSlim<TResult>
    {
        internal readonly TaskCompletionSource<TResult> _tcs;

        public TaskSourceSlim()
        {
            _tcs = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
    #endregion

    [Fact]
    public void WithPatternWorksWithRegularStructs()
    {
        var point = new Point(1, 2);
        // The property should be 'get; init;'
        // init-only setters is C# 9 feature.
        point = point with {X = 42};

        point.X.Should().Be(42);
    }

    // record structs are also available!

    #region Point

    public struct Point
    {
        public int X { get; init; }
        public int Y { get; init; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    #endregion

    #region Helpers
    private readonly ITestOutputHelper _output;

    public StructTests(ITestOutputHelper output)
    {
        _output = output;
    }
    #endregion
}