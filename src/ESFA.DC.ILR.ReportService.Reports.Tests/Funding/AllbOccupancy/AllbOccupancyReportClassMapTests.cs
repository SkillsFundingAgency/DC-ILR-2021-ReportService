using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.ALLB;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.AllbOccupancy
{
    public class AllbOccupancyReportClassMapTests
    {
        [Fact]
        public void Map_Columns()
        {
            var orderedColumns = new string[]
            {
                "Learner reference number",
                "Unique learner number",
                "Date of birth",
                "Pre-merger UKPRN",
                "Campus identifier",
                "Provider specified learner monitoring (A)",
                "Provider specified learner monitoring (B)",
                "Aim sequence number",
                "Learning aim reference",
                "Learning aim title",
                "Software supplier aim identifier",
                "Applicable funding rate",
                "Applicable programme weighting",
                "Notional NVQ level",
                "Tier 2 sector subject area",
                "Aim type",
                "Funding model",
                "Funding adjustment for prior learning",
                "Other funding adjustment",
                "Original learning start date",
                "Learning start date",
                "Learning planned end date",
                "Completion status",
                "Learning actual end date",
                "Outcome",
                "Learning delivery funding and monitoring type - Advanced Learner Loans indicator",
                "Learning delivery funding and monitoring type - Advanced Learner Loans Bursary funding",
                "Learning delivery funding and monitoring - ALB date applies from ",
                "Learning delivery funding and monitoring - ALB date applies to ",
                "Learning delivery funding and monitoring type - learning delivery monitoring (A)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (B)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (C)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (D)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (E)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (F)",
                "Provider specified delivery monitoring (A)",
                "Provider specified delivery monitoring (B)",
                "Provider specified delivery monitoring (C)",
                "Provider specified delivery monitoring (D)",
                "Sub contracted or partnership UKPRN",
                "Delivery location postcode",
                "Area uplift",
                "Funding line type",
                "First liability date",
                "Planned number of instalments",
                "Date used for factor lookups",
                "August ALB code used ",
                "August ALB support payment earned cash",
                "August loans bursary for area costs on programme earned cash",
                "August loans bursary for area costs balancing earned cash",
                "August loans bursary total earned cash",
                "September ALB code used ",
                "September ALB support payment earned cash",
                "September loans bursary for area costs on programme earned cash",
                "September loans bursary for area costs balancing earned cash",
                "September loans bursary total earned cash",
                "October ALB code used ",
                "October ALB support payment earned cash",
                "October loans bursary for area costs on programme earned cash",
                "October loans bursary for area costs balancing earned cash",
                "October loans bursary total earned cash",
                "November ALB code used ",
                "November ALB support payment earned cash",
                "November loans bursary for area costs on programme earned cash",
                "November loans bursary for area costs balancing earned cash",
                "November loans bursary total earned cash",
                "December ALB code used ",
                "December ALB support payment earned cash",
                "December loans bursary for area costs on programme earned cash",
                "December loans bursary for area costs balancing earned cash",
                "December loans bursary total earned cash",
                "January ALB code used ",
                "January ALB support payment earned cash",
                "January loans bursary for area costs on programme earned cash",
                "January loans bursary for area costs balancing earned cash",
                "January loans bursary total earned cash",
                "February ALB code used ",
                "February ALB support payment earned cash",
                "February loans bursary for area costs on programme earned cash",
                "February loans bursary for area costs balancing earned cash",
                "February loans bursary total earned cash",
                "March ALB code used ",
                "March ALB support payment earned cash",
                "March loans bursary for area costs on programme earned cash",
                "March loans bursary for area costs balancing earned cash",
                "March loans bursary total earned cash",
                "April ALB code used ",
                "April ALB support payment earned cash",
                "April loans bursary for area costs on programme earned cash",
                "April loans bursary for area costs balancing earned cash",
                "April loans bursary total earned cash",
                "May ALB code used ",
                "May ALB support payment earned cash",
                "May loans bursary for area costs on programme earned cash",
                "May loans bursary for area costs balancing earned cash",
                "May loans bursary total earned cash",
                "June ALB code used ",
                "June ALB support payment earned cash",
                "June loans bursary for area costs on programme earned cash",
                "June loans bursary for area costs balancing earned cash",
                "June loans bursary total earned cash",
                "July ALB code used ",
                "July ALB support payment earned cash",
                "July loans bursary for area costs on programme earned cash",
                "July loans bursary for area costs balancing earned cash",
                "July loans bursary total earned cash",
                "Total ALB support payment earned cash",
                "Total loans bursary for area costs on programme earned cash",
                "Total loans bursary for area costs balancing earned cash",
                "Total earned cash",
                "OFFICIAL - SENSITIVE",
            };

            var input = new List<AllbOccupancyReportModel>();

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<AllbOccupancyReportClassMap>();

                        csvWriter.WriteRecords(input);
                    }
                }

                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    using (var csvReader = new CsvReader(streamReader))
                    {
                        csvReader.Read();
                        csvReader.ReadHeader();
                        var header = csvReader.Context.HeaderRecord;
                        
                        header.Should().ContainInOrder(orderedColumns);
                        
                        header.Should().HaveCount(111);
                    }
                }
            }
        }

        [Fact]
        public void ClassMap_Model()
        {
            var input = new List<AllbOccupancyReportModel>()
            {
                new AllbOccupancyReportModel()
                {
                    Learner = new TestLearner()
                    {
                        LearnRefNumber = "Test"
                    },
                    LearningDelivery = new TestLearningDelivery(),
                    LarsLearningDelivery = new LARSLearningDelivery(),
                    ProviderSpecLearnerMonitoring = new ProviderSpecLearnerMonitoringModel(),
                    ProviderSpecDeliveryMonitoring = new ProviderSpecDeliveryMonitoringModel(),
                }
            };

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<AllbOccupancyReportClassMap>();

                        csvWriter.WriteRecords(input);
                    }
                }

                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    using (var csvReader = new CsvReader(streamReader))
                    {
                        var output = csvReader.GetRecords<dynamic>().ToList();

                        (output[0] as IDictionary<string, object>).Values.ToArray()[0].Should().Be("Test");
                    }
                }
            }
        }
    }
}
