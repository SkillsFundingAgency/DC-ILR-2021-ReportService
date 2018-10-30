using System.Linq;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public class MathsAndEnglishModelBuilder : IMathsAndEnglishModelBuilder
    {
        public MathsAndEnglishModel BuildModel(ILearner learner, FM25Learner fm25Data)
        {
            return new MathsAndEnglishModel
            {
                FundLine = fm25Data.FundLine,
                LearnRefNumber = learner.LearnRefNumber,
                FamilyName = learner.FamilyName,
                GivenNames = learner.GivenNames,
                DateOfBirth = learner.DateOfBirthNullable?.ToString("dd/MM/yyyy"),
                CampId = learner.CampId,
                ConditionOfFundingMaths = fm25Data.ConditionOfFundingMaths,
                ConditionOfFundingEnglish = fm25Data.ConditionOfFundingEnglish,
                RateBand = fm25Data.RateBand,
                ProvSpecLearnMonA = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "A")?.ProvSpecLearnMon,
                ProvSpecLearnMonB = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "B")?.ProvSpecLearnMon,
                LearnerStartDate = fm25Data?.LearnerStartDate
            };
        }
    }
}
