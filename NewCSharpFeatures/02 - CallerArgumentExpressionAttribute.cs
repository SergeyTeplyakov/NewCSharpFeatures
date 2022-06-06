using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Xunit;

namespace NewCSharpFeatures._02___CSharp_11;

/// <summary>
/// <see cref="CallerArgumentExpressionAttribute"/> allow developers to capture the expressions passed to a method,
/// to enable better error messages in diagnostic/testing APIs and reduce keystrokes.
///
/// Additional sources:
/// * https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/caller-argument-expression
/// </summary>
public class CallerArgumentExpressionAttributeSamples
{
    [Fact]
    public void ExplicitMessage()
    {
        int index = 0;
        
        // We can rely on the stacktrace when the error occurs.
        // Pros:
        //   - No extra code.
        // Cons:
        //   - If the code changes the line info can be incorrect.
        //   - Takes time to understand the nature of an error.
        Contract.Assert(0 <= index && index < 42);

        // Nice, but the comment might get stale and requires manual work to add it!
        Contract.Assert(0 <= index && index < 42, "0 <= index && index < 42");
    }

    [Fact]
    public void CompilerGeneratedExpressionNames()
    {
        int index = 0;
        string condition = MyAssert(0 <= index && index < 42);
        condition.Should().Be("0 <= index && index < 42");

        condition = MyAssert(index is >= 0 and < 42);
        condition.Should().Be("index is >= 0 and < 42");
    }

    private static string MyAssert(bool condition,
        [CallerArgumentExpression("condition")]string conditionText = "")
    {
        if (!condition)
        {
            // throw new ContractException(conditionText);
        }

        return conditionText;
    }

    // Use cases
    /*
     * - ArgumentNullException.ThrowIfNull
     * 1ES usages:
     *    - RuntimeContracts.
     *    - Non-null validators in AnyBuild (BuildXL and CloudBuild are next).
     */
    [Fact]
    public void NonNullValidation()
    {
        var config = new {ClientSettings = new {Communication = new {Port = (string?) null}}};

        var e = Assert.Throws<ArgumentNullException>(
            () => config.ClientSettings.Communication.Port.ThrowIfNull());
        e.ParamName.Should().Be("config.ClientSettings.Communication.Port");

        e = Assert.Throws<ArgumentNullException>(
            () => ArgumentNullException.ThrowIfNull(config.ClientSettings.Communication.Port));
        e.ParamName.Should().Be("config.ClientSettings.Communication.Port");
    }
}

public static class Guard
{
    [DebuggerStepThrough]
    public static T ThrowIfNull<T>([NotNull] this T? source, [CallerArgumentExpression("source")] string offendingNullArgumentName = "")
        where T : class
    {
        if (source == null)
        {
            throw new ArgumentNullException(offendingNullArgumentName);
        }

        return source;
    }
}