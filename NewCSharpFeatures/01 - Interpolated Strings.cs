using System.Runtime.CompilerServices;
using System.Text;
using FluentAssertions;
using Xunit;

namespace NewCSharpFeatures._02___CSharp_11;

/// <summary>
/// TLDR; Interpolated strings in C# 10 are more efficient and expressive than ever!
/// Platforms: Full Framework or .NET Core
///
/// Additional sources:
/// * https://sergeyteplyakov.github.io/Blog/c%2310/2021/11/08/Dissecing-Interpolated-Strings-Improvements-In-CSharp-10.html
/// * https://devblogs.microsoft.com/dotnet/string-interpolation-in-c-10-and-net-6/
/// </summary>
public class InterpolatedStringsImprovements
{
    [Fact]
    public void SimpleString()
    {
        // Creating a new string
        int n = 42;
        string s = $"n is {n}";

        // Translates to:
        // string s = string.Format("n == {0}", n);
        
        // Interpolated strings are convertible to 'string' or 'FormattableString'
        FormattableString ft = $"n is {n}";
    }

    // Conditional string construction
    [Fact]
    public void NonEfficientContracts()
    {
        int state = InitializeState();
        for (int i = 0; i < 10_000; i++)
        {
            // Quite expensive! Allocating a new string on every iteration regardless of the state!
            Contract.Assert(state == 42, $"_state must be 42, but was {state}.");

            // This is a real problem!
        }

        static int InitializeState() => 42;
    }


    // Interpolated String Handlers
    /*
     * C# 10 adds a family of types that control the way how the interpolated strings are
     * constructed.
     * The key type: `DefaultInterpolatedStringHandler`
     */
    [Fact]
    public void UsingDefaultInterpolatedStringHandler()
    {
        int n = 42;
        
        var s = $"n is {n}, {n}, {n}, {n}"; // s is string

        // C#1-9: var s = string.Format("n is {0}{1}{2}{3}", n, n, n, n);
        
        // C#10: 
        var h = new DefaultInterpolatedStringHandler(5, 1);
        h.AppendLiteral("n == ");
        h.AppendFormatted(n);
        string s_impl = h.ToStringAndClear();
    }

    // Does it matter?
    /*
     * - 10-40% performance improvements
     * - Reasonable reduction of allocations for normal cases
     * - No runtime work to parse the formatted string.
     * - No params allocations with 4 or more "captures":
     *   public static string FormatVersion(int major, int minor, int build, int revision) =>
            $"{major}.{minor}.{build}.{revision}";
     * - A generic overload of `AppendFormatted<T>(T)` to avoid boxing.
     * - Support for `ISpanFormattable` to append `Span<char>` without string allocations.
     * - Constant folding.
     */
    [Fact]
    public void ConstantFoldingAndReadOnlySupport()
    {
        // Span was not supported before!
        var span = new[] { 'a', 'b', 'c' }.AsSpan();
        const int n = 42;
        const string s = "s";
        var str = $"n is {n}, s is {s}, span is {span}";

        // Translates to: 
        var h = new DefaultInterpolatedStringHandler(22, 3);
        h.AppendLiteral("n is ");
        h.AppendFormatted(42);
        h.AppendLiteral(", s is ");
        h.AppendFormatted("s");
        h.AppendLiteral(", span is ");
        h.AppendFormatted((ReadOnlySpan<char>)span);
        string text = h.ToStringAndClear();
    }

    // Extensibility: custom Interpolated String Handlers
    /*
     * For lower framework version the attribute types must be added manually to the project!
     *
     * 'handlerIsValid' argument is critical!
     */
    [InterpolatedStringHandler]
    public ref struct ContractMessageInterpolatedStringHandler
    {
        // Will delegate all the work here!
        private DefaultInterpolatedStringHandler _handler;

        // The signature of this constructor is very important!
        // If it won't match the expected one, the compiler will emit an error.
        public ContractMessageInterpolatedStringHandler(int literalLength, int formattedCount, bool predicate, out bool handlerIsValid)
        {
            _handler = default;

            if (predicate)
            {
                // If the predicate is evaluated to 'true', then we don't have to construct a message!
                handlerIsValid = false;
                return;
            }

            handlerIsValid = true;
            _handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
        }

        public void AppendLiteral(string s) => _handler.AppendLiteral(s);

        public void AppendFormatted<T>(T t) => _handler.AppendFormatted(t);

        public override string ToString() => _handler.ToStringAndClear();
    }

    // "Telling" the compiler to pass the 'predicate' parameter to the handler.
    public static void MyAssert(bool predicate,
        [InterpolatedStringHandlerArgument("predicate")] ref ContractMessageInterpolatedStringHandler handler)
    {
        if (!predicate)
        {
            throw new Exception($"Precondition failed! Message:{handler.ToString()}");
        }
    }

    [Fact]
    public void LazyMessageConstruction()
    {
        int n = 0;
        Contract.Assert(true, $"Incrementing n. {n++}");
        n.Should().Be(1);

        MyAssert(true, $"Should not increment n. {n++}");
        n.Should().Be(1);
    }

    // How the laziness is implemented?
    [Fact]
    public void LazyMessageConstructionDecompilation()
    {
        int n = 0;
        MyAssert(true, $"Should not increment n. {n++}");

        bool flag = true;
        bool predicate = flag;
        var handler = new ContractMessageInterpolatedStringHandler(24, 1, flag, out var handlerIsValid);
        if (handlerIsValid)
        {
            // Creating a string only if the constructor returns true in 'handlerIsValid'!
            handler.AppendLiteral("Should not increment n. ");
            handler.AppendFormatted(n++);
        }

        MyAssert(predicate, ref handler);
    }

    // String builder use case
    [Fact]
    public void UsingInterpolatedStringsWithStringBuilder()
    {
        int n = 42;
        var sb = new StringBuilder();
        
        sb.Append($"x = {-1}, n = {n}.");
        
        // Using: public StringBuilder Append([InterpolatedStringHandlerArgument("")] ref AppendInterpolatedStringHandler handler) => this;
        // Which is very efficient and essentially the same as (but more efficient):
        sb.Append("x = ")
            .Append(-1)
            .Append(", n =")
            .Append(n)
            .Append(".");
    }

    // When to use this feature?
    /*
     * - Just use .net 6 and C# 10 to get a better performance!
     * - StringBuilder supports the new types natively.
     * - Add new types manually and target even lower framework versions (including Full Framework).
     * - Very useful for logging, guards (like Contracts).
     */
}