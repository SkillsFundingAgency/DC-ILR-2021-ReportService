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
            var returnModel = new HNSModel()
            {
                FundLine = fm25Data.FundLine,
                LearnRefNumber = learner.LearnRefNumber,
                FamilyName = learner.FamilyName,
                GivenNames = learner.GivenNames,
                CampId = learner.CampId
            };

            var learnerFAMs = learner.LearnerFAMs?.ToList();
            if (learnerFAMs != null)
            {
                returnModel.LearnerFAM_A = learner.LearnerFAMs.Any(x =>
                        string.Equals(x.LearnFAMType, "EHC", StringComparison.OrdinalIgnoreCase) &&
                        x.LearnFAMCode == 1)
                        ? "Y"
                        : "N";

                returnModel.LearnerFAM_B = learnerFAMs
                    .Any(x => string.Equals(x.LearnFAMType, "EHC", StringComparison.OrdinalIgnoreCase))
                    ? "N"
                    : "Y";

                returnModel.LearnerFAM_C = learnerFAMs.Any(x => !string.Equals(x.LearnFAMType, "EHC", StringComparison.OrdinalIgnoreCase) &&
                                                                (string.Equals(x.LearnFAMType, "HNS", StringComparison.OrdinalIgnoreCase) && x.LearnFAMCode == 1))
                    ? "Y"
                    : "N";

                returnModel.LearnerFAM_D = learnerFAMs.Any(x =>
                                                (string.Equals(x.LearnFAMType, "HNS", StringComparison.OrdinalIgnoreCase) ||
                                                string.Equals(x.LearnFAMType, "EHC", StringComparison.OrdinalIgnoreCase)) && x.LearnFAMCode == 1)
                    ? "Y"
                    : "N";

                returnModel.LearnerFAM_E = learnerFAMs.Any(x => !string.Equals(x.LearnFAMType, "HNS", StringComparison.OrdinalIgnoreCase) &&
                                                                (string.Equals(x.LearnFAMType, "EHC", StringComparison.OrdinalIgnoreCase) && x.LearnFAMCode == 1))
                    ? "Y"
                    : "N";
            }
            else
            {
                returnModel.LearnerFAM_A = "N";
                returnModel.LearnerFAM_B = "N";
                returnModel.LearnerFAM_C = "N";
                returnModel.LearnerFAM_D = "N";
                returnModel.LearnerFAM_E = "N";
            }

            return returnModel;
        }
    }
}
