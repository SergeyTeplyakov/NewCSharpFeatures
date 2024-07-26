using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace NewCSharpFeatures._02___CSharp_11._02_Improving_Expressiveness;

// More information: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns

public class TypeChecks(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void BasicTypeCheck()
    {
        object obj = "Hello, World!";
        if (obj is string s)
        {
            testOutputHelper.WriteLine(s);
        }
    }

    [Fact]
    public void TypeCheckWithTypeHierarchy()
    {
        GetCount(new int[]{1, 2, 3}).Should().Be(3);
        GetCount(new List<int> {1, 2, 3}).Should().Be(3);
        GetCount(Enumerable.Range(1, 3)).Should().Be(3);

        static int GetCount<T>(IEnumerable<T> sequence)
        {
            int length = 0;
            switch (sequence)
            {
                case int[] array:
                    length = array.Length;
                    break;
                case ICollection<int> c:
                    length = c.Count;
                    break;
                default:
                    length = sequence.Count();
                    break;
            }

            return length;
        }
    }
}

public class SwitchExpressions
{
    [Fact]
    public void TypeCheckWithTypeHierarchy()
    {
        GetCount(new int[] { 1, 2, 3 }).Should().Be(3);
        GetCount(new List<int> { 1, 2, 3 }).Should().Be(3);
        GetCount(Enumerable.Range(1, 3)).Should().Be(3);

        static int GetCount<T>(IEnumerable<T> sequence)
        {
            // Convert to switch expression
            int length = 0;
            switch (sequence)
            {
                case int[] array:
                    length = array.Length;
                    break;
                case ICollection<int> c:
                    length = c.Count;
                    break;
                default:
                    length = sequence.Count();
                    break;
            }

            return length;
        }
    }
}

#region Using Conditions

public abstract record Vehicle(int Weight) { }
public record Car(int Weight) : Vehicle(Weight) { }
public record Truck(int Weight) : Vehicle(Weight) { }

public class ConditionsInSwitch
{
    public static int CalculateToll(Vehicle vehicle)
    {
        return vehicle switch
        {
            Car c when c.Weight < 1000 => 2,
            Car => 5,
            Truck t when t.Weight < 1000 => 10,
            Truck => 20,
            _ => throw new InvalidOperationException($"Unknown type {vehicle.GetType()}")
        };
    }
}
#endregion Using Conditions

public class ListPatterns(ITestOutputHelper output)
{
    [Fact]
    public void Example()
    {
        int[] numbers = { 1, 2, 3 };

        output.WriteLine((numbers is [1, 2, 3]).ToString());  // True
        output.WriteLine((numbers is [1, 2, 4]).ToString());  // False
        output.WriteLine((numbers is [1, 2, 3, 4]).ToString());  // False
        
        // Really!?!?!?
        output.WriteLine((numbers is [0 or 1, <= 2, >= 3]).ToString());  // True
    }
}
