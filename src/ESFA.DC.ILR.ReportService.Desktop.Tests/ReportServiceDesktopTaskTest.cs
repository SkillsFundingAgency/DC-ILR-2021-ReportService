using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Desktop.Context;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Desktop.Tests
{
    public class ReportServiceDesktopTaskTest
    {
        [Fact]
        public async Task ExecuteAsync()
        {
            var cancellationToken = CancellationToken.None;

            var builder = new ContainerBuilder();
            builder.RegisterCommonServiceStubs();

            var desktopContextMock = new Mock<IDesktopContext>();

            var reportServiceContextFactoryMock = new Mock<IReportServiceContextFactory<IDesktopContext>>();
            var reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextFactoryMock.Setup(f => f.Build(desktopContextMock.Object)).Returns(reportServiceContextMock.Object);

            var entryPointMock = new Mock<IEntryPoint>();
            entryPointMock.Setup(s => s.Callback(reportServiceContextMock.Object, cancellationToken)).Returns(Task.FromResult(new List<string>())).Verifiable();

            builder.RegisterInstance(entryPointMock.Object).As<IEntryPoint>();            
            var container = builder.Build();

            var lifetimeScope = container.Resolve<ILifetimeScope>();         

            var result = await NewTask(lifetimeScope, reportServiceContextFactoryMock.Object).ExecuteAsync(desktopContextMock.Object, cancellationToken);

            result.Should().BeSameAs(desktopContextMock.Object);

            entryPointMock.VerifyAll();
        }

        [Theory(Skip = "This needs review, not really a unit test")]
        [InlineData("TaskGenerateValidationReport")]
        [InlineData("TaskGenerateValidationSchemaErrorsReport")]
        public async Task TestValidationReports_EndToEnd(string validationReportName)
        {
            var cancellationToken = CancellationToken.None;

            var container = DIComposition.BuildContainer().Build();
            var entryPoint = container.Resolve<IEntryPoint>();
            Mock<IDesktopContext> mockDesktopContext = new Mock<IDesktopContext>();
            var keyValuePairs = new Dictionary<string, object>
            {
                { "Filename", "ILR-10033670-1819-20190617-102124-03.xml" },
                { "OriginalFilename", "mtheoriginal.xml"},
                { "FileSizeInBytes", 128},
                { "Container", @"TestFiles\"},
                { "IlrReferenceData", @"IlrReferenceData.json"},
                { "ValidationErrors", "10033670_99_ValidationErrors"},
                { "ValidLearnRefNumbersCount", 1},
                { "InvalidLearnRefNumbersCount", 2},
                { "ValidationTotalErrorCount", 3},
                { "ValidationTotalWarningCount", 4},
                { "ValidationErrorLookups", "ValidationErrorLookups"},
                { "FundingFm81Output", "FundingFm81Output"},
                { "FundingFm70Output", "FundingFm70Output"},
                { "FundingFm36Output", "FundingFm36Output"},
                { "FundingFm35Output", "FundingFm35Output"},
                { "FundingFm25Output", "FundingFm25Output"},
                { "FundingAlbOutput", "FundingAlbOutput"},
                { "ValidLearnRefNumbers", "ValidLearnRefNumbers"},
                { "ReturnPeriod", 8},
                { "ReportTasks", validationReportName}
            };
            mockDesktopContext.Setup(x => x.DateTimeUtc).Returns(new DateTime(2019, 06, 25));
            mockDesktopContext.SetupGet(x => x.KeyValuePairs).Returns(keyValuePairs);

            var reportServiceJobContextDesktopContext = new ReportServiceJobContextDesktopContext(mockDesktopContext.Object, Enumerable.Empty<IReportFilterQuery>());

            var reportOutputFileNames = await entryPoint.Callback(reportServiceJobContextDesktopContext, cancellationToken);

            reportOutputFileNames.Should().NotBeNullOrEmpty();
            reportOutputFileNames.Count.Should().Be(1);
            reportServiceJobContextDesktopContext.ReportOutputFileNames.Should().NotBeNullOrEmpty();
            reportServiceJobContextDesktopContext.ReportOutputFileNames.Split('|').ToArray().Length.Should().Be(1);
        }

        private ContainerBuilder BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterCommonServiceStubs();
            //builder.RegisterModule<ReportServiceDesktopModule>();

            var zipServiceMock = new Mock<IZipService>();
            zipServiceMock.Setup(x => x.CreateZipAsync(
                It.IsAny<IReportServiceContext>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return builder;
        }

        private ReportServiceDesktopTask NewTask(ILifetimeScope lifetimeScope = null, IReportServiceContextFactory<IDesktopContext> reportServiceContextFactory = null)
        {
            return new ReportServiceDesktopTask(lifetimeScope ?? Mock.Of<ILifetimeScope>(), reportServiceContextFactory ?? new Mock<IReportServiceContextFactory<IDesktopContext>>().Object);
        }
    }
}
