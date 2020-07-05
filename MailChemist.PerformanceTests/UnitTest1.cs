using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Xunit;

namespace MailChemist.PerformanceTests
{
    [MemoryDiagnoser]
    public class UnitTest1
    {
        [GlobalSetup]
        public void Setup()
        {
            string xx = "";
        }

        [Benchmark]
        public void Test1()
        {
            string x = "";
        }
    }

    public class Test
    {
        [Fact]
        public void Binding()
        {
            BenchmarkRunner.Run<UnitTest1>();
        }
    }
}