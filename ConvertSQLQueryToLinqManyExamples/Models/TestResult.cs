using Newtonsoft.Json;
using System;

namespace ConvertSQLQueryToLinqManyExamples.Models
{
    public class TestResult { 
        
        public string Name; 
        public bool Result; 
        public string Output; 
        public string Expected;
        public TimeSpan ElapsedTime;
    }

    public class TestResult<T> : TestResult
    {
        [JsonIgnore]
        public T ResultObject;
        public TestResult( string name, bool result, string output, string expected, T resultObject, TimeSpan elapsedTime )
        {
            Name = name;
            Result = result;
            Output = output;
            Expected = expected;
            ResultObject = resultObject;
            ElapsedTime = elapsedTime;
        }
    }

}
