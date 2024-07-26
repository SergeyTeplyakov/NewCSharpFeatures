
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace NewCSharpFeatures._02___CSharp_11;

#region The basics
#nullable disable
public class OldPerson
{
    // By default, since C# 1.0, null was a valid value for any reference types.
    // Can 'Children' be null?
    // Can the value in the List<string> be null?
    public List<string> Children { get; set; }
}

public class OldPersonTests
{
    [Fact]
    public void Nullability()
    {
        // Tony Hoare called inventing null references his billion-dollar mistake.
        var p = new OldPerson();
        p.Children = null;
        p.Children = new List<string>();
        p.Children.Add(null);
    }
}

#nullable enable
/*
Studies had shown that in vast majority of cases, the reference type members are not null.

So C# 8 (when non-nullable types are enabled) flips the defaults: reference types can't be null by default!
*/

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

public class PersonTests
{
    public void Test()
    {
        var p = new Person();
        Console.WriteLine(p.Children.Count); // Safe
        
        if (p.JobTitle != null)
        {
            Console.WriteLine(p.JobTitle.Length); // Warning, JobTitle can be null
        }

        foreach(var friend in p.Friends) // Warning : Friends can be null
        {
            // Warning: friend can be null
            Console.WriteLine(friend.Length);
        }
    }
}

#endregion The basics

#region Nullability and runtime checks

public class NullabilityAndRuntimeChecks()
{
#nullable disable
    private static string GetValue() => null;
#nullable restore

    public void FooBar(string name) // C# 8. name is not nullable.
    // this is a public api.
    {
        // name
        ArgumentNullException.ThrowIfNull(name);
    }

    [Fact]
    public void StillWillFail()
    {
        // no warnings!
        // 'GetValue' is nullable-oblivious since it comes from the "old" code.
        string value = GetValue();

        // NullReferenceException!
        Console.WriteLine(value.Length);
    }
}
#endregion Nullability and runtime checks

// Nullability can be opt-in: per file or per project bases.

// Proposed guidelines:
// Enable non-nullable types for all new projects
// Fixing any NullReferenceException should begin with enabling non-nullable types in the file.
//
// Start from the bottom: the code that is used the most would have the biggest impact.
// use '#nullable enable' in a file
// When all the files are migrated use '<Nullable>enable</Nullable>' in CSProj file.

#region .NET Framework Challenges

// The code is not annotated.
public class NullabilityExample
{
    #nullable disable
    public static bool IsNullOrEmpty(string str) => string.IsNullOrEmpty(str);
    #nullable enable

    [Fact]
    public void Test()
    {
        SumLength(null, null);
    }

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
}

public static class NullabilityExtensions
{
    public static string NotNull(this string? str) => str ?? throw new ArgumentNullException();
}

#endregion .NET Framework Challenges

#region Advanced techniques

/*
 * There are a lot of attributes for controlling nullability.
 */

public class Cache<T> where T : class
{
    private readonly Dictionary<string, T> _cache = new();

    // value is not null if the method returns true.
    public bool TryGetValue(string key, [NotNullWhen(true)]out T? value)
    {
        return _cache.TryGetValue(key, out value);
    }
}

public static class CacheUsage
{
    private static void UsingCache()
    {
        var cache = new Cache<string>();
        if (cache.TryGetValue("key", out var value))
        {
            // value is not null here.
            Console.WriteLine(value.Length);
        }
        else
        {
            // Warning: dereferencing a possibly null reference
            Console.WriteLine(value.Length);
        }
    }
}

public record Error(Exception Exception);

public static class Result
{
    public static Result<TResult> Success<TResult>(TResult value) => new Result<TResult>(value, error: null);
    public static Result<TResult> Error<TResult>(Error error) => new Result<TResult>(value: default, error);
}

public class Result<TResult>
{
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Succeeded { get; }
    
    public TResult? Value { get; }

    public Error? Error { get; }

    internal Result(TResult? value, Error? error)
    {
        Debug.Assert(value is not null || error is not null);
        Succeeded = value is not null;
        Value = value;
        Error = error;
    }
}

public class UsingResults
{
    public void ProcessResults(Result<string> result)
    {
        if (result.Succeeded)
        {
            // 'result.Value' is not null here.
            Console.WriteLine(result.Value.Length);
        }
        else
        {
            // 'result.Error' is not null here.
            Console.WriteLine(result.Error.Exception.Message);
        }
    }
}


#endregion Advanced techniques

// Other nullability attributes:
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
// [DisallowNull], [AllowNull], [NotNull], [MaybeNull], [NotNullWhen], [NotNullIfNotNull], [DoesNotReturnIf], [DoesNotReturn]