using System.Diagnostics.CodeAnalysis;

using Xunit;
using Xunit.Abstractions;

namespace NewCSharpFeatures._02___CSharp_11._02_Improving_Expressiveness;

public class Binary_Separator(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void DigitSeparator()
    {
        int a = 1_000_000;
        uint b = 0x0FFFF_0000;
        byte c = 0b1001_1001;

        testOutputHelper.WriteLine(a.ToString());
        testOutputHelper.WriteLine(b.ToString("X"));
        testOutputHelper.WriteLine(c.ToString("b8"));
    }
}

public class RawStringLiterals
{
    [Fact]
    public void Test()
    {
        ProcessXml("""
                   <element attr="content">
                       <body>
                       </body>
                   </element>
                   """);

        ProcessJson("""
                    "x": 1,
                    "y": [1, 2, 3]
                    """);
        ProcessRegex("\\G");
    }

    private static void ProcessXml([StringSyntax(StringSyntaxAttribute.Xml)] string str) { }

    private static void ProcessJson([StringSyntax(StringSyntaxAttribute.Json)] string str) { }

    private static void ProcessRegex([StringSyntax(StringSyntaxAttribute.Regex)] string str) { }
}

public class FunWithRanges(ITestOutputHelper output)
{
    #region Fun With Ranges
    [Fact]
    public void MoreFunWithRanges()
    {
        // Similar to foreach (var n in Enumerable.Range(1, 10))
        // DO NOT USE IT IN PRODUCTION CODE! :)

        foreach (var n in 1..10)
        {
            output.WriteLine(n.ToString());
        }
    }
    #endregion Fun With Ranges

}

public class AwaitUsing
{
    public async Task<string> DispatchAsync()
    {
        using SemaphoreSlim semaphore = new(1);
        await using SemaphoreLease lease = await SemaphoreLease.AcquireAsync(semaphore);
        return await ProcessAsync();
    }

    private async Task<string> ProcessAsync()
    {
        await Task.Delay(1000);
        return "Done";
    }

    // private static async
    public class SemaphoreLease : IAsyncDisposable
    {
        public static async Task<SemaphoreLease> AcquireAsync(SemaphoreSlim semaphore)
        {
            return new SemaphoreLease();
        }

        public ValueTask DisposeAsync()
        {
            // Doing other work.
            return new ValueTask();
        }
    }
}

static class RangesExtensions
{
    public static IEnumerator<int> GetEnumerator(this Range range)
    {
        for (int i = range.Start.Value; i <= range.End.Value; i++)
        {
            yield return i;
        }
    }
}