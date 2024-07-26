using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

using FluentAssertions;
using Xunit;

namespace NewCSharpFeatures._02___CSharp_11._02_Improving_Expressiveness;

public static class Contract
{
    public static string Assert(
        [DoesNotReturnIf(false)]
        bool condition,
        [InterpolatedStringHandlerArgument("condition")] ref ContractMessageInterpolatedStringHandler userMessage,
        [CallerArgumentExpression("condition")] string conditionText = ""
        )
    {
        if (!condition)
        {
            ReportFailure(conditionText, userMessage.ToString());
        }

        return conditionText;
    }

    private static void ReportFailure(string conditionText, string userMessage)
    {
        string message = $"Contract violation: {conditionText}. {userMessage}";
        throw new ContractException(message);
    }

    public class ContractException : Exception
    {
        public ContractException(string? message) : base(message)
        {
        }
    }
}

public class ContractViolationExamples
{
    [Fact]
    public void ShowUserMessage()
    {
        int x = 42;
        int y = -1;
        // Contract violation: x != 42 && y != -1. x: 42, y: -1
        Contract.Assert(x != 42 && y != -1, $"x: {x}, y: {y}");
    }

    [Fact]
    public void LazyEvaluation()
    {
        int sideEffectTracker = 0;
        for (int i = 0; i < 1000; i++)
        {
            // We won't evaluate the expression!
            // More info: https://sergeyteplyakov.github.io/Blog/c%2310/2021/11/08/Dissecing-Interpolated-Strings-Improvements-In-CSharp-10.html
            Contract.Assert(i >= 0, $"i: {i}. {sideEffectTracker++}");
        }

        sideEffectTracker.Should().Be(0);
    }
}

[InterpolatedStringHandler]
public readonly ref struct ContractMessageInterpolatedStringHandler
{
    /// <summary>
    /// Lazily allocated string builder for creating an error string.
    /// </summary>
    /// <remarks>
    /// It's fine to use a newly created builder each time when the assertion is violated, because we know that it should not be happening very often.
    /// </remarks>
    private readonly StringBuilder? _builder;

    /// <summary>
    /// A constructor that the compiler calls for creating an interpolated string.
    /// </summary>
    public ContractMessageInterpolatedStringHandler(int literalLength, int formattedCount, bool predicate, out bool handlerIsValid)
    {
        // Completely ignoring the first two arguments that are required by the compiler.
        // But we don't need them because the contract violations must happen very infrequently and the fact that the string construction in that case is not
        // super efficient is not important.
        _builder = null;

        if (predicate)
        {
            // The assersion is not violated. Not creating a string at all.
            handlerIsValid = false;
            return;
        }

        handlerIsValid = true;
        _builder = new StringBuilder();
    }

    /// <summary>
    /// Appends a given <paramref name="s"/> into a final message.
    /// </summary>
    public void AppendLiteral(string s) => _builder!.Append(s);

    /// <summary>
    /// Appends a given <paramref name="t"/> to a final message.
    /// </summary>
    public void AppendFormatted<T>(T t) => _builder!.Append(t?.ToString());

    /// <inheritdoc />
    public override string ToString() => _builder!.ToString();
}