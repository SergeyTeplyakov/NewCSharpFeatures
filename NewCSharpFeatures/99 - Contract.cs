namespace NewCSharpFeatures._02___CSharp_11;

public class Contract
{
    public static void Assert(bool condition, string? message = null)
    {
        if (!condition)
        {
            throw new Exception(message ?? "Contract failure");
        }
    }

    public class ContractException : Exception
    {
        public ContractException(string? message) : base(message)
        {
        }
    }
}