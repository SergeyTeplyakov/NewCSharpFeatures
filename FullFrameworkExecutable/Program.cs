using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

public record MyRecord
{
    // System.Runtime.CompilerServices.IsExternalInit class is required.
    public int X { get; init; }
}

//namespace System.Runtime.CompilerServices
//{
//    internal class IsExternalInit { }
//}

internal class Program
{
    static void Main(string[] args)
    {
        var r = new MyRecord() {X = 42};
        Console.WriteLine(r);
    }
}