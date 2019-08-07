using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Trailblazer;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.TrailblazerOccupancy
{
    public class TrailblazerOccupancyReportClassMapTests
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
                "Notional NVQ level",
                "Aim type",
                "Apprenticeship standard code",
                "Funding model",
                "Funding adjustment for prior learning",
                "Other funding adjustment",
                "Original learning start date",
                "Learning start date",
                "Learning planned end date",
                "Completion status",
                "Learning actual end date",
                "Outcome",
                "Achievement date",
                "Learning delivery funding and monitoring type - source of funding",
                "Learning delivery funding and monitoring type - eligibility for enhanced apprenticeship funding",
                "Learning delivery funding and monitoring type - learning support funding (highest applicable)",
                "Learning delivery funding and monitoring - LSF date applies from (earliest) ",
                "Learning delivery funding and monitoring - LSF date applies to (latest)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (A)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (B)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (C)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (D)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (E)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (F)",
                "Learning delivery funding and monitoring type - restart indicator",
                "Provider specified delivery monitoring (A)",
                "Provider specified delivery monitoring (B)",
                "Provider specified delivery monitoring (C)",
                "Provider specified delivery monitoring (D)",
                "End point assessment organisation",
                "Sub contracted or partnership UKPRN",
                "Delivery location postcode",
                "LARS maximum core government contribution (£)",
                "LARS small employer incentive (£)",
                "LARS 16-18 year-old apprentice incentive (£)",
                "LARS achievement incentive (£)",
                "Applicable funding value date",
                "Funding line type",
                "Employer identifier on first day of standard",
                "Employer identifier on small employer threshold date",
                "Employer identifier on first 16-18 threshold date",
                "Employer identifier on second 16-18 threshold date",
                "Employer identifier on achievement date",
                "Start indicator for maths, English and learning support",
                "Age at start of standard",
                "Eligible for 16-18 year-old apprentice incentive",
                "Eligible for small employer incentive",
                "Applicable achievement date",
                "Latest total negotiated price (TNP) 1 (£)",
                "Latest total negotiated price (TNP) 2 (£)",
                "Sum of PMRs before this funding year (£)",
                "Sum of August payment records (PMRs) (£)",
                "August core government contribution (£)",
                "August maths and English on-programme earned cash (£)",
                "August maths and English balancing earned cash (£)",
                "August learning support earned cash (£)",
                "August small employer incentive (£)",
                "August 16-18 year-old apprentice incentive (£)",
                "August achievement incentive (£)",
                "Sum of September payment records (PMRs) (£)",
                "September core government contribution (£)",
                "September maths and English on-programme earned cash (£)",
                "September maths and English balancing earned cash (£)",
                "September learning support earned cash (£)",
                "September small employer incentive (£)",
                "September 16-18 year-old apprentice incentive (£)",
                "September achievement incentive (£)",
                "Sum of October payment records (PMRs) (£)",
                "October core government contribution (£)",
                "October maths and English on-programme earned cash (£)",
                "October maths and English balancing earned cash (£)",
                "October learning support earned cash (£)",
                "October small employer incentive (£)",
                "October 16-18 year-old apprentice incentive (£)",
                "October achievement incentive (£)",
                "Sum of November payment records (PMRs) (£)",
                "November core government contribution (£)",
                "November maths and English on-programme earned cash (£)",
                "November maths and English balancing earned cash (£)",
                "November learning support earned cash (£)",
                "November small employer incentive (£)",
                "November 16-18 year-old apprentice incentive (£)",
                "November achievement incentive (£)",
                "Sum of December payment records (PMRs) (£)",
                "December core government contribution (£)",
                "December maths and English on-programme earned cash (£)",
                "December maths and English balancing earned cash (£)",
                "December learning support earned cash (£)",
                "December small employer incentive (£)",
                "December 16-18 year-old apprentice incentive (£)",
                "December achievement incentive (£)",
                "Sum of January payment records (PMRs) (£)",
                "January core government contribution (£)",
                "January maths and English on-programme earned cash (£)",
                "January maths and English balancing earned cash (£)",
                "January learning support earned cash (£)",
                "January small employer incentive (£)",
                "January 16-18 year-old apprentice incentive (£)",
                "January achievement incentive (£)",
                "Sum of February payment records (PMRs) (£)",
                "February core government contribution (£)",
                "February maths and English on-programme earned cash (£)",
                "February maths and English balancing earned cash (£)",
                "February learning support earned cash (£)",
                "February small employer incentive (£)",
                "February 16-18 year-old apprentice incentive (£)",
                "February achievement incentive (£)",
                "Sum of March payment records (PMRs) (£)",
                "March core government contribution (£)",
                "March maths and English on-programme earned cash (£)",
                "March maths and English balancing earned cash (£)",
                "March learning support earned cash (£)",
                "March small employer incentive (£)",
                "March 16-18 year-old apprentice incentive (£)",
                "March achievement incentive (£)",
                "Sum of April payment records (PMRs) (£)",
                "April core government contribution (£)",
                "April maths and English on-programme earned cash (£)",
                "April maths and English balancing earned cash (£)",
                "April learning support earned cash (£)",
                "April small employer incentive (£)",
                "April 16-18 year-old apprentice incentive (£)",
                "April achievement incentive (£)",
                "Sum of May payment records (PMRs) (£)",
                "May core government contribution (£)",
                "May maths and English on-programme earned cash (£)",
                "May maths and English balancing earned cash (£)",
                "May learning support earned cash (£)",
                "May small employer incentive (£)",
                "May 16-18 year-old apprentice incentive (£)",
                "May achievement incentive (£)",
                "Sum of June payment records (PMRs) (£)",
                "June core government contribution (£)",
                "June maths and English on-programme earned cash (£)",
                "June maths and English balancing earned cash (£)",
                "June learning support earned cash (£)",
                "June small employer incentive (£)",
                "June 16-18 year-old apprentice incentive (£)",
                "June achievement incentive (£)",
                "Sum of July payment records (PMRs) (£)",
                "July core government contribution (£)",
                "July maths and English on-programme earned cash (£)",
                "July maths and English balancing earned cash (£)",
                "July learning support earned cash (£)",
                "July small employer incentive (£)",
                "July 16-18 year-old apprentice incentive (£)",
                "July achievement incentive (£)",
                "Total payment records (PMRs) for this funding year (£)",
                "Total core government contribution (£)",
                "Total maths and English on-programme earned cash (£)",
                "Total maths and English balancing earned cash (£)",
                "Total learning support earned cash (£)",
                "Total small employer incentive (£)",
                "Total 16-18 year-old apprentice incentive (£)",
                "Total achievement incentive (£)",
                "OFFICIAL - SENSITIVE",
            };

            var input = new List<TrailblazerOccupancyReportModel>();

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<TrailblazerOccupancyReportClassMap>();

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

                        header.Should().HaveCount(167);
                    }
                }
            }
        }

        [Fact]
        public void ClassMap_Model()
        {
            var input = new List<TrailblazerOccupancyReportModel>()
            {
                new TrailblazerOccupancyReportModel()
                {
                    Learner = new TestLearner()
                    {
                        LearnRefNumber = "Test"
                    },
                    LearningDelivery = new TestLearningDelivery(),
                    LarsLearningDelivery = new LARSLearningDelivery(),
                    ProviderSpecLearnerMonitoring = new ProviderSpecLearnerMonitoringModel(),
                    ProviderSpecDeliveryMonitoring = new ProviderSpecDeliveryMonitoringModel(),
                    AppFinRecord = new AppFinRecordModel(),
                }
            };

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<TrailblazerOccupancyReportClassMap>();

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
