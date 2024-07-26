using System.Linq;

using Xunit;
using Xunit.Abstractions;

namespace NewCSharpFeatures._02___CSharp_11._02_Improving_Expressiveness;

public class Collections(ITestOutputHelper output)
{
    [Fact]
    public void Basics()
    {
        // C# 1: manual collection initialization
        var list = new List<int>();
        list.Add(1);
        list.Add(2);

        // C# 3: using collection initializers
        list = new List<int> {1, 2};

        // C# 9: using target-typed new
        list = new() {1, 2};

        // C# 12: collection expression
        list = [1, 2];

        list = [..list, 3, 4, 5]; // list is [1,2,3,4,5] now.

        int[] a = []; // An empty array! Similar to 'Array.Empty<int>'.
    }

    [Fact]
    public void CollectionExpressionsDeepDive()
    {
        // Collection expressions can't be used with 'var'.
        // Collection expression can be converted to various target types
        // and that type should be provided explicitly.
        // Error: There is no target type for the collection expression.
        // var list = [1, 2];

        // Simple array.
        int[] a = [1, 2];

        // Uses simple array, gets the underlying span of it, and fills it with data.
        // More optimal than calling 'List.Add'.
        List<int> b = [1, 2];

        // No heap allocations: uses a readonly blob from the assembly.
        ReadOnlySpan<int> c = [1, 2];

        // No heap allocations 
        Span<int> d = [1, 2];
        
        // var tmp = new int[] {1, 2};
        // var d = new ReadOnlyArray<int>(tmp);
        // ReadOnlyArray is a private class generated by the compiler.
        IEnumerable<int> e = [1, 2];
    }

    [Fact]
    public void CheckIEnumerable()
    {
        IEnumerable<int> data = [1, 2, 3];
        output.WriteLine(data.GetType().ToString());
    }
}

public class IndicesAndRanges(ITestOutputHelper output)
{
    private int[] Ids = [
        // index from start     index from end
        1, // 0                    ^10
        2, // 1                    ^9
        3, // 2                    ^8
        4, // 3                    ^7
        5, // 4                    ^6
        6, // 5                    ^5
        7, // 6                    ^4
        8, // 7                    ^3
        9, // 8                    ^2
        10 // 9                    ^1
           //                      ^0 -> the item after the last one
    ];

    [Fact]
    public void TestIndicesAndRanges()
    {
        // Syntax: [x..y)
        // x - inclusive, y - exclusive
        // [0..^0] -> the entire range.
        output.WriteLine(Ids[0..5].AsString()); // 1,2,3,4,5
        
        // ..5 is the same as 0..5
        output.WriteLine(Ids[..5].AsString()); // 1,2,3,4,5

        // ..5 is the same as 0..5
        output.WriteLine(Ids[1..5].AsString()); // 2,3,4,5

        // The last item:
        output.WriteLine(Ids[^1].ToString()); // 10
        
        // The last two items
        output.WriteLine(Ids[^2..].AsString()); // 9,10
        output.WriteLine(Ids[^2..^0].AsString()); // 9,10
    }
}

static class StringExtensions
{
    public static string AsString(this int[] a) => string.Join(",", a);
}