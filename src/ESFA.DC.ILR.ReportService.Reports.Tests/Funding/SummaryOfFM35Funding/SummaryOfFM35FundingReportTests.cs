using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.SummaryOfFM35Funding
{
    public class SummaryOfFM35FundingReportTests
    {
        [Fact]
        public async Task GenerateAsync()
        {
            var container = "Container";

            var reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.SetupGet(c => c.Container).Returns(container);

            var reportServiceDependentData = Mock.Of<IReportServiceDependentData>();
            var cancellationToken = CancellationToken.None;
            var fileName = "FileName";
            var model = new List<SummaryOfFM35FundingReportModel>();

            var fileNameServiceMock = new Mock<IFileNameService>();

            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "Summary of Funding Model 35 Funding Report", OutputTypes.Csv, true)).Returns(fileName);

            var modelBuilderMock = new Mock<IModelBuilder<IEnumerable<SummaryOfFM35FundingReportModel>>>();

            modelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData)).Returns(model);

            var csvServiceMock = new Mock<ICsvService>();

            var result = await NewReport(fileNameServiceMock.Object, modelBuilderMock.Object, csvServiceMock.Object).GenerateAsync(reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);

            csvServiceMock.Verify(s => s.WriteAsync<SummaryOfFM35FundingReportModel, SummaryOfFM35FundingReportClassMap>(model, fileName, container, cancellationToken));

            result.First().Should().Be(fileName);
            result.Should().HaveCount(1);
        }
        
        private SummaryOfFM35FundingReport NewReport(
            IFileNameService fileNameService = null,
            IModelBuilder<IEnumerable<SummaryOfFM35FundingReportModel>> modelBuilder = null,
            ICsvService csvService = null)
        {
            return new SummaryOfFM35FundingReport(fileNameService, modelBuilder, csvService);
        }
    }
}
