using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using static NewCSharpFeatures._02___CSharp_11.PatternMatchingSamples.State;
using static NewCSharpFeatures._02___CSharp_11.PatternMatchingSamples.Transition;

namespace NewCSharpFeatures._02___CSharp_11;

/// <summary>
/// Pattern matching is a technique where you test an expression
/// to determine if it has certain characteristics.
/// </summary>
public class PatternMatchingSamples
{
    [Fact]
    public void SimpleChecks()
    {
        // An object with a nested structure
        var config = 
            new { ClientSettings = new { Communication = new { Port = (int?)42, Description = "Description" } } };

        // Null checking for value and reference types.
        if (config.ClientSettings.Communication.Port is int p)
        {
            _output.WriteLine($"The port is {p}");
        }

        // Introducing an alias for a property and checking it to null.
        if (config.ClientSettings.Communication is { } c)
        {
            _output.WriteLine(c.Description);
        }
    }

    [Fact]
    public void ValueMatching()
    {
        var data = (port: (int?)42, description: "Description");

        // 'is' can be used to match the exact value.
        if (data.port is 42 or > 40)
        {
            _output.WriteLine("data.port is 42 or > 40");
        }

        // 'is' can be used with more complex expressions as well.
        if (data is (42, "Description"))
        {
            _output.WriteLine(data.ToString());
        }
    }

    [Fact]
    public void FunWithTuples()
    {
        State current = Locked;
        Transition transition = Lock;
        bool hasKey = true;
        
        var newState = (current, transition) switch
        {
            // using static for enums.
            (Opened, Close) => Closed,
            (Closed, Open) => Opened,
            (Closed, Lock) when hasKey => Locked,
            (Closed, Unlock) when hasKey => Closed,
            _ => throw new InvalidOperationException("Invalid transition")
        };

        // Tuple deconstruction.
        var (success, errorMessage) = PerformOperation();

        static (bool success, string errorMessage) PerformOperation() => default;
    }

    #region States
    public enum State { Opened, Closed, Locked }
    public enum Transition { Close, Open, Lock, Unlock }
    #endregion

    [Fact]
    public void TypeTestsAndExtendedProperties()
    {
        object o = string.Empty;
        // A simple type test
        if (o is string s1)
        {
            _output.WriteLine(s1);
        }

        // C# 10: extended property patterns. A super fancy way for NullOrEmpty checks.
        
        if (o is string { Length: > 0} s)
        {
            _output.WriteLine(s);
        }

        // Using records with a more complicated patterns
        var command = new Command(CommandType.ReImage, Data: "//MachineName");
        if (command is {Type: CommandType.ReImage, Data: string d} && d.StartsWith("//"))
        {
            _output.WriteLine(command.ToString());
        }
    }

    #region Command and CommandType
    public enum CommandType { ReImage, Build }
    public record Command(CommandType Type, object Data);

    #endregion

    [Fact]
    public void TypeTestsInSwitch()
    {
        Result result = PerformOperation();

        switch (result)
        {
            // Add when exceptionFailure.Exception is InvalidOperationException
            case ExceptionFailure exceptionFailure when exceptionFailure.Exception is InvalidOperationException:
                _output.WriteLine($"Exception: {exceptionFailure.Exception}");
                break;
            case Failure failure:
                _output.WriteLine($"Error: {failure.Error}");
                break;
            case Success:
                _output.WriteLine("Success");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
        static Result PerformOperation() => new Success();
    }

    #region Results

    public abstract record Result(bool Success);
    public record Success() : Result(Success: true) { }
    public record Failure(string Error) :Result(Success: false) { }
    public record ExceptionFailure(Exception Exception) : Failure(Error: Exception.ToString()) {}
    #endregion

    #region Ctor
    private readonly ITestOutputHelper _output;

    public PatternMatchingSamples(ITestOutputHelper output)
    {
        _output = output;
    }
    #endregion
}