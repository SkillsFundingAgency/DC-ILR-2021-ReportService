using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.DevolvedOccupancy
{
    public class DevolvedAdultEducationOccupancyReportTests
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
            var model = new List<DevolvedAdultEducationOccupancyReportModel>();

            var fileNameServiceMock = new Mock<IFileNameService>();

            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "Devolved Adult Education Occupancy Report", OutputTypes.Csv, true)).Returns(fileName);

            var modelBuilderMock = new Mock<IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>>>();

            modelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData)).Returns(model);

            var csvServiceMock = new Mock<ICsvService>();

            var result = await NewReport(fileNameServiceMock.Object, modelBuilderMock.Object, csvServiceMock.Object).GenerateAsync(reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);

            csvServiceMock.Verify(s => s.WriteAsync<DevolvedAdultEducationOccupancyReportModel, DevolvedAdultEducationOccupancyReportClassMap>(model, fileName, container, cancellationToken));

            result.First().Should().Be(fileName);
            result.Should().HaveCount(1);
        }
        
        private DevolvedAdultEducationOccupancyReport NewReport(
            IFileNameService fileNameService = null,
            IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>> devolvedAdultEducationOccupancyReportModelBuilder = null,
            ICsvService csvService = null)
        {
            return new DevolvedAdultEducationOccupancyReport(fileNameService, devolvedAdultEducationOccupancyReportModelBuilder, csvService);
        }
    }
}
