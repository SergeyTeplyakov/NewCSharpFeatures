using Xunit;

namespace NewCSharpFeatures._02___CSharp_11.Records;

// Records
public record Person(string FirstName, string LastName);

// Can be a class or a struct as well.
// Not fully bulletproof, since older C# versions might still be able to mutate the properties
// and construct the instance without setting them.
public record Person2
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}

public class BasicTests
{
    [Fact]
    public void Immutability()
    {
        // var p = new Person { FirstName = "John", LastName = "Doe" };
        var p = new Person("John", "Doe");

        //var p = new Person()
        //{
        //    FirstName = "John",
        //    LastName = "Doe"
        //};
        // p.FirstName = "Jane"; // Error: Property or indexer 'Person.FirstName' cannot be assigned to -- it is read-only

        var p2 = new Person2()
        {
            FirstName = "John",
            LastName = "Doe",
        };

        // Older C# version would allow this:
        // Person2 p = new Person2();
        // p.FirstName = "Jane"; // Error by the compiler that recognises the init-only property.
    }
}

#region Different types of immutability

public class Person3
{
    // The field is readonly, the property is get-only.

    public string FirstName { get; }

    // The property is get-only and must be set in the constructor.
    public string LastName { get; }

    // The property must be set in the constructor or in object initializer.
    public required int Age { get; init; }

    public Person3(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}

public class PersonTests
{
    [Fact]
    public void Person3Test()
    {
        var p = new Person3("John", "Doe")
        {
            // Commenting the setter will break the compilation.
            Age = 42
        };
    }
}

#endregion Different types of immutability