using Newtonsoft.Json;
using System;

namespace ConvertSQLQueryToLinqManyExamples
{
    class Program
    {
        static void Main( string[] args )
        {
            var testService = new MetaServiceTest();
            Console.Write( JsonConvert.SerializeObject(testService.RunTests(), Formatting.Indented) );
            Console.WriteLine();
            Console.WriteLine("Press any key to close the console.");
            Console.ReadKey();
        }
    }
}
