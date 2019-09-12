using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.NonContractedAppsActivity
{
    public class NonContractedAppsActivityReportModelBuilderTests
    {
        [Fact]
        public void ValidContractMappings()
        {
            var mappings = new List<KeyValuePair<string, string[]>>
            {
                new KeyValuePair<string, string[]>(FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 }),
                new KeyValuePair<string, string[]>(FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.C1618nlap2018 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 })
            };

            NewReport().ValidContractMappings.Should().BeEquivalentTo(mappings);
        }

        [Fact]
        public void BuildLarsLearningDeliveryDictionary()
        {
            var refData = new ReferenceDataRoot
            {
                LARSLearningDeliveries = new List<LARSLearningDelivery>
                {
                    new LARSLearningDelivery
                    {
                        LearnAimRef = "LearnAimRef1",
                        LearnAimRefTitle = "Title1"
                    },
                    new LARSLearningDelivery
                    {
                        LearnAimRef = "LearnAimRef2",
                        LearnAimRefTitle = "Title2"
                    }
                }
            };

            NewReport().BuildLarsLearningDeliveryDictionary(refData).Should().HaveCount(2);
            NewReport().BuildLarsLearningDeliveryDictionary(refData).Should().ContainKeys(new string[] { "LearnAimRef1", "LearnAimRef2" });
        }


        private NonContractedAppsActivityReportModelBuilder NewReport(IIlrModelMapper ilrModelMapper = null)
        {
            return new NonContractedAppsActivityReportModelBuilder(ilrModelMapper);
        }
    }
}
