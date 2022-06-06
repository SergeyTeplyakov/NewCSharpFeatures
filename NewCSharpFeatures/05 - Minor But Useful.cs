using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;

namespace NewCSharpFeatures._02___CSharp_11;

public class MinorButUsefulFeatures
{
    [Fact]
    public void DigitSeparator()
    {
        int a = 1_000_000;
        uint b = 0x0FFFF_0000;
        byte c = 0b1001_1001;
    }

    [Fact]
    public void RangesBasics()
    {
        // See System.Range for more information
        var a = new int[] {1, 2, 3, 4, 5};
        
        Print(a[1..3]); // 2, 3
        Print(a[2..^1]); // 3, 4
        Print(a[..^1]); // 1, 2, 3, 4
    }

    [Fact]
    public void MoreFunWithRanges()
    {
        // Similar to foreach (var n in Enumerable.Range(1, 10))
        // DO NOT USE IT IN PRODUCTION CODE! :)
        
        foreach (var n in 1..10)
        {
            _output.WriteLine(n.ToString());
        }
    }

    [Fact]
    public void ImplicitConversionToDelegates()
    {
        // This was not working before!
        // Due to the minimal API requirements the lambda conversion to the delegates was added.
        // var result = source.Where(id == 42);
        
        var x = () => true;

        // Still works!
        Expression<Func<bool>> e = () => true;
    }

    [Fact]
    public void StaticLambdas()
    {
        // A very important feature for high performance code!
        var cd = new ConcurrentDictionary<string, int>();
        int valueToAdd = 42;
        cd.GetOrAdd(
            key: "key",
            valueFactory: (key, valueToAdd) =>
            {
                return valueToAdd;
            },
            factoryArgument: valueToAdd);
    }

    [Fact]
    public void Discards()
    {
        var cd = new ConcurrentDictionary<string, int>();
        int valueToAdd = 42;

        _ = cd.AddOrUpdate(
            key: "key",
            addValueFactory: static (key /*_*/, valueToAdd) =>
            {
                return valueToAdd;
            },
            updateValueFactory: static (key, existingValue, valueToAdd) =>
            {
                return valueToAdd;
            },
            factoryArgument: valueToAdd);

        // Events.
        BackgroundWorker bw = new BackgroundWorker();
        
        // ¯\_(ツ)_/¯
        bw.DoWork += (_, _) => { };

        static void DoStuff(string arg)
        {
            // Dubious but possible
            _ = arg ?? throw new ArgumentNullException(arg);
        }
    }

    #region Helpers
    void Print(IEnumerable<int> data)
    {
        _output.WriteLine(string.Join(", ", data.Select(d => d.ToString())));
    }

    private readonly ITestOutputHelper _output;

    public MinorButUsefulFeatures(ITestOutputHelper output)
    {
        _output = output;
    }
    #endregion

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