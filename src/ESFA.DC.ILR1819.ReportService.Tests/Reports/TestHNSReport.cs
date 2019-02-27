using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.ILR1819.ReportService.Tests.Helpers;
using ESFA.DC.ILR1819.ReportService.Tests.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Moq;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.Reports
{
    public class TestHNSReport
    {
        [InlineData("ILR-10033670-1819-20180704-120055-03.xml", "ValidLearnRefNumbers.json", "Fm25.json")]
        public async Task TestHNSReportGeneration(string ilrFilename, string validLearnRefNumbersFilename, string fm25Filename)
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            IFM25ProviderService fm25ProviderService = new FM25ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, intUtilitiesService, dataStoreConfiguration);

            var hnsReport = new HNSDetailReport(
                logger.Object,
                storage.Object,
                ilrProviderService,
                stringUtilitiesService,
                validLearnersService,
                fm25ProviderService,
                dateTimeProviderMock.Object,
                valueProvider,
                topicAndTaskSectionOptions,
                reportModelBuilder);

            await hnsReport.GenerateReport(reportServiceContextMock.Object, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();

#if DEBUG
            File.WriteAllText($"{filename}.csv", csv);
#endif

            TestCsvHelper.CheckCsv(csv, new CsvEntry(new HNSMapper(), 1));
        }
    }
}
