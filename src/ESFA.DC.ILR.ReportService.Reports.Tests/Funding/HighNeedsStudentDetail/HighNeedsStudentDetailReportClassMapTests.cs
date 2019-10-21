using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentDetail;
using ESFA.DC.ILR.ReportService.Reports.Tests.Abstract;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.HighNeedsStudentDetail
{
    public class HighNeedsStudentDetailReportClassMapTests : AbstractCsvClassMapTests<HighNeedsStudentDetailReportModel, HighNeedsStudentDetailReportClassMap>
    {
        protected override IEnumerable<string> OrderedColumns => new[]
        {
            "Funding line type",
            "Learner reference number",
            "Family name",
            "Given names",
            "Campus identifier",
            "A - Students with an EHCP",
            "B - Students without an EHCP",
            "C - High Needs Students (HNS) without an EHCP",
            "D - Students with an EHCP and HNS",
            "E - Students with an EHCP but without HNS",
            "OFFICIAL-SENSITIVE",
        };
        
        [Fact]
        public void ClassMap_Model()
        {
            var input = new List<HighNeedsStudentDetailReportModel>()
            {
                new HighNeedsStudentDetailReportModel()
                {
                    Learner = new TestLearner()
                    {
                        LearnRefNumber = "LearnRefNumber",
                        FamilyName = "FamilyName",
                        GivenNames = "GivenNames",
                        CampId = "CampId",
                    },
                    FM25Learner = new FM25Learner()
                    {
                        FundLine = "FundLine"
                    },
                    StudentsWithAnEhcp = true,
                    StudentsWithoutAnEhcp = true,
                    HighNeedsStudentsWithoutAnEhcp = true,
                    StudentsWithAnEhcpAndHns = true,
                    StudentWithAnEhcpAndNotHns = true,
                }
            };

            var output = (WriteAndReadModel(input).ToList().First() as IDictionary<string, object>).Values.ToArray();

            output[0].Should().Be("FundLine");
            output[1].Should().Be("LearnRefNumber");
            output[2].Should().Be("FamilyName");
            output[3].Should().Be("GivenNames");
            output[4].Should().Be("CampId");
            output[5].Should().Be("Y");
            output[6].Should().Be("Y");
            output[7].Should().Be("Y");
            output[8].Should().Be("Y");
            output[9].Should().Be("Y");
            output[10].Should().Be(string.Empty);
        }
    }
}
