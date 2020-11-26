﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.AppsIndicitave;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.AppsIndicativeEarnings
{
    public class AppsIndicativeEarningsReportTests
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
            var model = new List<AppsIndicativeEarningsReportModel>();

            var fileNameServiceMock = new Mock<IFileNameService>();

            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "Apps Indicative Earnings Report", OutputTypes.Csv, true)).Returns(fileName);

            var modelBuilderMock = new Mock<IModelBuilder<IEnumerable<AppsIndicativeEarningsReportModel>>>();

            modelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData)).Returns(model);

            var csvServiceMock = new Mock<ICsvFileService>();

            var result = await NewReport(fileNameServiceMock.Object, modelBuilderMock.Object, csvServiceMock.Object).GenerateAsync(reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);

            csvServiceMock.Verify(s => s.WriteAsync<AppsIndicativeEarningsReportModel, AppsIndicativeEarningsReportClassMap>(model, fileName, container, cancellationToken, null, null));

            result.First().Should().Be(fileName);
            result.Should().HaveCount(1);
        }
        
        private AppsIndicativeEarningsReport NewReport(
            IFileNameService fileNameService = null,
            IModelBuilder<IEnumerable<AppsIndicativeEarningsReportModel>> modelBuilder = null,
            ICsvFileService csvService = null)
        {
            return new AppsIndicativeEarningsReport(fileNameService, modelBuilder, csvService);
        }
    }
}
