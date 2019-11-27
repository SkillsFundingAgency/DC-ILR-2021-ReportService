using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.AppsIndicitave;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.AppsIndicitave.Model;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.AppsIndicativeEarnings
{
    public class NonContractedAppsActivityReportClassMapTests
    {
        [Fact]
        public void Map_Columns()
        {
            var orderedColumns = new string[]
            {
                "Learner reference number",
                "Unique learner number",
                "Date of birth",
                "Postcode prior to enrolment",
                "Campus identifier",
                "Provider specified learner monitoring (A)",
                "Provider specified learner monitoring (B)",
                "Aim sequence number",
                "Learning aim reference",
                "Learning aim title",
                "Software supplier aim identifier",
                "LARS 16-18 framework uplift",
                "Notional NVQ level",
                "Standard notional end level",

                "Tier 2 sector subject area",
                "Programme type",
                "Standard code",
                "Framework code",
                "Apprenticeship pathway",
                "Aim type",
                "Common component code",
                "Funding model",
                "Original learning start date",
                "Learning start date",
                "Learning planned end date",
                "Completion status",
                "Learning actual end date",
                "Achievement date",
                "Outcome",
                "Funding adjustment for prior learning",
                "Other funding adjustment",
                "Learning delivery funding and monitoring type - learning support funding (if applicable)",
                "Learning delivery funding and monitoring type - LSF date applies from (earliest)",
                "Learning delivery funding and monitoring type - LSF date applies to (latest)",
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
                "Planned number of on programme instalments for aim",
                "Sub contracted or partnership UKPRN",
                "Delivery location postcode",
                "Employer identifier on employment status date",
                "Agreement identifier",
                "Employment status",
                "Employment status date",
                "Employment status monitoring - small employer",
                "Price episode start date",
                "Price episode actual end date",

                "Funding line type",
                "Total price applicable to this episode",
                "Funding band upper limit",
                "Price amount above funding band limit",
                "Price amount remaining (with upper limit applied) at start of this episode",
                "Completion element (potential or actual earnings)",
                "Total employer contribution collected (PMR) in previous funding years",
                "Total employer contribution collected (PMR) in this funding year",

                "Learning delivery funding and monitoring type - apprenticeship contract type",
                "Learning delivery funding and monitoring type - ACT date applies from",
                "Learning delivery funding and monitoring type - ACT date applies to",

                "August on programme earnings",
                "August balancing payment earnings",
                "August aim completion earnings",
                "August learning support earnings",
                "August English and maths on programme earnings",
                "August English and maths balancing payment earnings",
                "August disadvantage earnings",
                "August 16-18 additional payments for employers",
                "August 16-18 additional payments for providers",
                "August additional payments for apprentices",
                "August 16-18 framework uplift on programme payment",
                "August 16-18 framework uplift balancing payment",
                "August 16-18 framework uplift completion payment",

                "September on programme earnings",
                "September balancing payment earnings",
                "September aim completion earnings",
                "September learning support earnings",
                "September English and maths on programme earnings",
                "September English and maths balancing payment earnings",
                "September disadvantage earnings",
                "September 16-18 additional payments for employers",
                "September 16-18 additional payments for providers",
                "September additional payments for apprentices",
                "September 16-18 framework uplift on programme payment",
                "September 16-18 framework uplift balancing payment",
                "September 16-18 framework uplift completion payment",

                "October on programme earnings",
                "October balancing payment earnings",
                "October aim completion earnings",
                "October learning support earnings",
                "October English and maths on programme earnings",
                "October English and maths balancing payment earnings",
                "October disadvantage earnings",
                "October 16-18 additional payments for employers",
                "October 16-18 additional payments for providers",
                "October additional payments for apprentices",
                "October 16-18 framework uplift on programme payment",
                "October 16-18 framework uplift balancing payment",
                "October 16-18 framework uplift completion payment",

                "November on programme earnings",
                "November balancing payment earnings",
                "November aim completion earnings",
                "November learning support earnings",
                "November English and maths on programme earnings",
                "November English and maths balancing payment earnings",
                "November disadvantage earnings",
                "November 16-18 additional payments for employers",
                "November 16-18 additional payments for providers",
                "November additional payments for apprentices",
                "November 16-18 framework uplift on programme payment",
                "November 16-18 framework uplift balancing payment",
                "November 16-18 framework uplift completion payment",

                "December on programme earnings",
                "December balancing payment earnings",
                "December aim completion earnings",
                "December learning support earnings",
                "December English and maths on programme earnings",
                "December English and maths balancing payment earnings",
                "December disadvantage earnings",
                "December 16-18 additional payments for employers",
                "December 16-18 additional payments for providers",
                "December additional payments for apprentices",
                "December 16-18 framework uplift on programme payment",
                "December 16-18 framework uplift balancing payment",
                "December 16-18 framework uplift completion payment",

                "January on programme earnings",
                "January balancing payment earnings",
                "January aim completion earnings",
                "January learning support earnings",
                "January English and maths on programme earnings",
                "January English and maths balancing payment earnings",
                "January disadvantage earnings",
                "January 16-18 additional payments for employers",
                "January 16-18 additional payments for providers",
                "January additional payments for apprentices",
                "January 16-18 framework uplift on programme payment",
                "January 16-18 framework uplift balancing payment",
                "January 16-18 framework uplift completion payment",

                "February on programme earnings",
                "February balancing payment earnings",
                "February aim completion earnings",
                "February learning support earnings",
                "February English and maths on programme earnings",
                "February English and maths balancing payment earnings",
                "February disadvantage earnings",
                "February 16-18 additional payments for employers",
                "February 16-18 additional payments for providers",
                "February additional payments for apprentices",
                "February 16-18 framework uplift on programme payment",
                "February 16-18 framework uplift balancing payment",
                "February 16-18 framework uplift completion payment",

                "March on programme earnings",
                "March balancing payment earnings",
                "March aim completion earnings",
                "March learning support earnings",
                "March English and maths on programme earnings",
                "March English and maths balancing payment earnings",
                "March disadvantage earnings",
                "March 16-18 additional payments for employers",
                "March 16-18 additional payments for providers",
                "March additional payments for apprentices",
                "March 16-18 framework uplift on programme payment",
                "March 16-18 framework uplift balancing payment",
                "March 16-18 framework uplift completion payment",

                "April on programme earnings",
                "April balancing payment earnings",
                "April aim completion earnings",
                "April learning support earnings",
                "April English and maths on programme earnings",
                "April English and maths balancing payment earnings",
                "April disadvantage earnings",
                "April 16-18 additional payments for employers",
                "April 16-18 additional payments for providers",
                "April additional payments for apprentices",
                "April 16-18 framework uplift on programme payment",
                "April 16-18 framework uplift balancing payment",
                "April 16-18 framework uplift completion payment",

                "May on programme earnings",
                "May balancing payment earnings",
                "May aim completion earnings",
                "May learning support earnings",
                "May English and maths on programme earnings",
                "May English and maths balancing payment earnings",
                "May disadvantage earnings",
                "May 16-18 additional payments for employers",
                "May 16-18 additional payments for providers",
                "May additional payments for apprentices",
                "May 16-18 framework uplift on programme payment",
                "May 16-18 framework uplift balancing payment",
                "May 16-18 framework uplift completion payment",

                "June on programme earnings",
                "June balancing payment earnings",
                "June aim completion earnings",
                "June learning support earnings",
                "June English and maths on programme earnings",
                "June English and maths balancing payment earnings",
                "June disadvantage earnings",
                "June 16-18 additional payments for employers",
                "June 16-18 additional payments for providers",
                "June additional payments for apprentices",
                "June 16-18 framework uplift on programme payment",
                "June 16-18 framework uplift balancing payment",
                "June 16-18 framework uplift completion payment",

                "July on programme earnings",
                "July balancing payment earnings",
                "July aim completion earnings",
                "July learning support earnings",
                "July English and maths on programme earnings",
                "July English and maths balancing payment earnings",
                "July disadvantage earnings",
                "July 16-18 additional payments for employers",
                "July 16-18 additional payments for providers",
                "July additional payments for apprentices",
                "July 16-18 framework uplift on programme payment",
                "July 16-18 framework uplift balancing payment",
                "July 16-18 framework uplift completion payment",

                "Total on programme earnings",
                "Total balancing payment earnings",
                "Total aim completion earnings",
                "Total learning support earnings",
                "Total English and maths on programme earnings",
                "Total English and maths balancing payment earnings",
                "Total disadvantage earnings",
                "Total 16-18 additional payments for employers",
                "Total 16-18 additional payments for providers",
                "Total additional payments for apprentices",
                "Total 16-18 framework uplift on programme payment",
                "Total 16-18 framework uplift balancing payment",
                "Total 16-18 framework uplift completion payment",

                "OFFICIAL - SENSITIVE",
            };

            var input = new List<AppsIndicativeEarningsReportModel>();

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<AppsIndicativeEarningsReportClassMap>();

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
                        header.Should().HaveCount(237);
                    }
                }
            }
        }

        [Fact]
        public void ClassMap_Model()
        {
            var input = new List<AppsIndicativeEarningsReportModel>()
            {
                new AppsIndicativeEarningsReportModel()
                {
                    Learner = new TestLearner()
                    {
                        LearnRefNumber = "Test",
                        DateOfBirthNullable = new DateTime(2019,07,13)
                    },
                    LearningDelivery = new TestLearningDelivery()
                    {
                        OrigLearnStartDateNullable = null
                    },
                    LarsLearningDelivery = new LARSLearningDelivery(),
                    ProviderSpecLearnerMonitoring = new ProviderSpecLearnerMonitoringModel(),
                    ProviderSpecDeliveryMonitoring = new ProviderSpecDeliveryMonitoringModel(),
                    PeriodisedValues = new PeriodisedValuesModel()
                    {
                        August = new PeriodisedValuesAttributesModel()
                        {
                            AdditionalPaymentForEmployers1618 = (decimal?) 1.1
                        }
                    }
                }
            };

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<AppsIndicativeEarningsReportClassMap>();
                        csvWriter.Configuration.TypeConverterOptionsCache.GetOptions<DateTime?>().Formats = new[] { "dd/MM/yyyy" };

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
                        (output[0] as IDictionary<string, object>).Values.ToArray()[2].Should().Be("13/07/2019");
                        (output[0] as IDictionary<string, object>).Values.ToArray()[22].Should().Be(""); // OriginalLearnStartDate
                        (output[0] as IDictionary<string, object>).Values.ToArray()[74].Should().Be("1.10000");
                    }
                }
            }
        }
    }
}
