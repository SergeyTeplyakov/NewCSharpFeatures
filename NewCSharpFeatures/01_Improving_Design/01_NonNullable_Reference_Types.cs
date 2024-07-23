
namespace NewCSharpFeatures._02___CSharp_11;

#nullable disable
public class OldPerson
{
    // By default since C# 1.0, null was a valid value for any reference types.
    // Can 'Children' be null?
    // Can the value in the List<string> be null?
    public List<string> Children { get; }
}

#nullable enable
public class Person
{
    // Children property is not null.
    // The values in the list are not nullable as well.
    public List<string> Children { get; } = new();
    
    // The JobTitle is optional
    public string? JobTitle { get; } = default;
    
    // 'Friends' property can be null.
    // The items in 'Friends' property can be null as well.
    public List<string?>? Friends { get; } = default;

    // Warning: a key in the dictionary can't be null!
    public Dictionary<string?, int> FriendsWithAge { get; } = new();
}

#region .NET Framework Challenges

// The code is not annotated.
public static class NullabilityExample
{
    #nullable disable
    public static bool IsNullOrEmpty(string str) => string.IsNullOrEmpty(str);
    #nullable enable

    public static int SumLength(string? str1, string? str2)
    {
        int result = 0;
        if (!string.IsNullOrEmpty(str1))
        {
            // Fine
            result += str1.Length;
        }

        if (!IsNullOrEmpty(str2))
        {
            // Warning: str2 might be null.
            result += str2.Length;

            // Solution: custom helpers.
            result += str2.NotNull().Length;
            // Not recommended: null-forgiving operator.
            result += str2!.Length;
        }

        return result;
    }

    public static T NotNull<T>(this T? t) => t!;
}

#endregion  .NET Framework Challenges

public class CanBeNullOrNot
{
    public static void ShowNullability()
    {
        // Since C# 1.0 null was a valid value for any reference type receiver:
        #nullable disable
        
        // Totally valid!
        string str = null;
        //var myInstance = new {X = null,}
        #nullable restore
    }
}

