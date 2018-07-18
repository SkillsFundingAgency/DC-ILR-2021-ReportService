using Autofac;
using ESFA.DC.ILR1819.ReportService.Stateless;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests
{
    public sealed class AutoFacTest
    {
        [Fact]
        public void TestRegistrations()
        {
            ContainerBuilder container = Program.BuildContainer(new TestConfigurationHelper());
            container.Build();
        }
    }
}
