using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public sealed class HNSReportModelBuilder : IHNSReportModelBuilder
    {
        public HNSModel BuildModel(ILearner learner, ILearningDelivery learningDelivery, FM25Learner fm25Data)
        {
            return new HNSModel()
            {
                FundLine = fm25Data.FundLine,
                LearnRefNumber = learner.LearnRefNumber,
                FamilyName = learner.FamilyName,
                GivenNames = learner.GivenNames,
                CampId = learner.CampId,
                LearnerFAM_A = (learner.LearnerFAMs?.Where(x => string.Equals(x.LearnFAMType, "EHC", StringComparison.OrdinalIgnoreCase) && x.LearnFAMCode == 1)).Any() ? "Y" : "N",
            };
        }

        private string returnLearnerFAM(List<ILearnerFAM> learnerFAMs, string learFAMtype, int learnFAMCode)
        {
            if (learnerFAMs != null)
            {
            }

            return "N";
        }
    }
}
