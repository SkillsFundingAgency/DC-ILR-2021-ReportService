using System;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Builders.PeriodEnd;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels.PeriodEnd;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders.PeriodEnd
{
    public class AppsAdditionalPaymentsModelBuilder : IAppsAdditionalPaymentsModelBuilder
    {
        public AppsAdditionalPaymentsModel BuildModel(ILearner learner, FM36Learner learnerData)
        {
            // get variables of monthly earnings here and a total
            // get variables of monthly payments here and a total
            return new AppsAdditionalPaymentsModel
            {
                LearnerReferenceNumber = learner.LearnRefNumber,
                UniqueLearnerNumber = learner.ULN,
                ProviderSpecifiedLearnerMonitoringA = learner.ProviderSpecLearnerMonitorings?.SingleOrDefault(psm =>
                    string.Equals(psm.ProvSpecLearnMonOccur, "A", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                ProviderSpecifiedLearnerMonitoringB = learner.ProviderSpecLearnerMonitorings?.SingleOrDefault(psm =>
                    string.Equals(psm.ProvSpecLearnMonOccur, "B", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                LearningStartDate = DateTime.MinValue, // todo Requires DAS data
                FundingLineType = string.Empty, // todo Requires DAS data
                TypeOfAdditionalPayment = string.Empty, // todo Requires DAS data
                EmployerNameFromApprenticeshipService = string.Empty, // todo Requires DAS data
                EmployerIdentifierFromILR = string.Empty, // todo Requires DAS data
                AugustEarnings = 0, // todo Requires DAS data
                AugustR01Payments = 0, // todo Requires DAS data
                SeptemberEarnings = 0, // todo Requires DAS data
                SeptemberR02Payments = 0, // todo Requires DAS data
                OctoberEarnings = 0, // todo Requires DAS data
                OctoberR03Payments = 0, // todo Requires DAS data
                NovemberEarnings = 0, // todo Requires DAS data
                NovemberR04Payments = 0, // todo Requires DAS data
                DecemberEarnings = 0, // todo Requires DAS data
                DecemberR05Payments = 0, // todo Requires DAS data
                JanuaryEarnings = 0, // todo Requires DAS data
                JanuaryR06Payments = 0, // todo Requires DAS data
                FebruaryEarnings = 0, // todo Requires DAS data
                FebruaryR07Payments = 0, // todo Requires DAS data
                MarchEarnings = 0, // todo Requires DAS data
                MarchR08Payments = 0, // todo Requires DAS data
                AprilEarnings = 0, // todo Requires DAS data
                AprilR09Payments = 0, // todo Requires DAS data
                MayEarnings = 0, // todo Requires DAS data
                MayR10Payments = 0, // todo Requires DAS data
                JuneEarnings = 0, // todo Requires DAS data
                JuneR11Payments = 0, // todo Requires DAS data
                JulyEarnings = 0, // todo Requires DAS data
                JulyR12Payments = 0, // todo Requires DAS data
                R13Payments = 0, // todo Requires DAS data
                R14Payments = 0, // todo Requires DAS data
                TotalEarnings = 0, // todo Requires DAS data
                TotalPaymentsYearToDate = 0 // todo Requires DAS data
            };
        }
    }
}
