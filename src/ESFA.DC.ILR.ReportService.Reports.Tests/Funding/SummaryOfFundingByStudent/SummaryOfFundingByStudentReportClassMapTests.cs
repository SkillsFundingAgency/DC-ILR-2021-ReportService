using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.SummaryOfFundingByStudent;
using ESFA.DC.ILR.ReportService.Reports.Tests.Abstract;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.SummaryOfFundingByStudent
{
    public class SummaryOfFundingByStudentReportClassMapTests : AbstractCsvClassMapTests<SummaryOfFundingByStudentReportModel, SummaryOfFundingByStudentReportClassMap>
    {
        protected override IEnumerable<string> OrderedColumns => new[]
        {
            "Funding line type",
            "Learner reference number",
            "Family name",
            "Given names",
            "T Level student",
            "Date of birth",
            "Campus identifier",
            "Planned learning hours",
            "Planned employability, enrichment and pastoral hours",
            "Total planned hours",
            "Planned hours (T Level)",
            "Funding band",
            "Qualifies for funding",
            "Total funding",
            "OFFICIAL-SENSITIVE",
        };
        
        [Fact]
        public void ClassMap_Model()
        {
            var input = new List<SummaryOfFundingByStudentReportModel>()
            {
                new SummaryOfFundingByStudentReportModel()
                {
                    Learner = new TestLearner()
                    {
                        LearnRefNumber = "Test",
                        FamilyName = "FamilyName",
                        GivenNames = "GivenNames",
                        DateOfBirthNullable = new System.DateTime(2002, 1, 1),
                        CampId = "123",
                    },
                    FM25Learner = new FM25Learner
                    {
                        FundLine = "FundLine",
                        TLevelStudent = true,
                        StartFund = false,
                        RateBand = "Band",
                        OnProgPayment = 100.2m
                    },
                    NonTLevelPlanLearnHours = 1,
                    NonTLevelPlanEEPHours = 2,
                    TLevelPlannedHours = 3,
                    NonTLevelTotalPlannedHours = 4
                }
            };

            var output = (WriteAndReadModel(input).ToList().First() as IDictionary<string, object>).Values.ToArray();

            output[0].Should().Be("FundLine");
            output[1].Should().Be("Test");
            output[2].Should().Be("FamilyName");
            output[3].Should().Be("GivenNames");
            output[4].Should().Be("Y");
            output[5].Should().Be("01/01/2002");
            output[6].Should().Be("123");
            output[7].Should().Be("1");
            output[8].Should().Be("2");
            output[9].Should().Be("4");
            output[10].Should().Be("3");
            output[11].Should().Be("Band");
            output[12].Should().Be("N");
            output[13].Should().Be("100.2");
            output[14].Should().Be(string.Empty);
        }
    }
}
