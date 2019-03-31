using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Builders.PeriodEnd;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
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

        public List<AppsAdditionalPaymentsModel> BuildModel(
            AppsAdditionalPaymentILRInfo appsAdditionalPaymentIlrInfo,
            AppsAdditionalPaymentRulebaseInfo appsAdditionalPaymentRulebaseInfo,
            AppsAdditionalPaymentDasPaymentsInfo appsAdditionalPaymentDasPaymentsInfo)
        {
            List<AppsAdditionalPaymentsModel> appsAdditionalPaymentsModels = new List<AppsAdditionalPaymentsModel>();

            //var paymentGroups = appsAdditionalPaymentDasPaymentsInfo.Payments
            //    .GroupBy(p => new
            //    {
            //        p.UkPrn,
            //        p.LearnerReferenceNumber,
            //        p.LearningAimReference,
            //        p.LearningStartDate,
            //        p.LearningAimProgrammeType,
            //        p.LearningAimStandardCode,
            //        p.LearningAimFrameworkCode,
            //        p.LearningAimPathwayCode
            //    });

            foreach (var paymentInfo in appsAdditionalPaymentDasPaymentsInfo.Payments)
            {
                foreach (var learner in appsAdditionalPaymentIlrInfo.Learners)
                {
                    var appsAdditionalPaymentLearningDeliveryInfo = learner.LearningDeliveries?.SingleOrDefault(x => x.UKPRN == paymentInfo.UkPrn &&
                                                                                                                     x.LearnRefNumber ==
                                                                                                                     paymentInfo.LearnerReferenceNumber &&
                                                                                                                     x.LearnAimRef == paymentInfo.LearningAimReference &&
                                                                                                                     x.LearnStartDate == paymentInfo.LearningStartDate &&
                                                                                                                     x.ProgType == paymentInfo.LearningAimProgrammeType &&
                                                                                                                     x.StdCode == paymentInfo.LearningAimStandardCode &&
                                                                                                                     x.FworkCode == paymentInfo.LearningAimFrameworkCode &&
                                                                                                                     x.PwayCode == paymentInfo.LearningAimPathwayCode);
                    var aecLearningDeliveryInfo = appsAdditionalPaymentRulebaseInfo.AECLearningDeliveries.SingleOrDefault(x =>
                        x.UKPRN == appsAdditionalPaymentLearningDeliveryInfo.UKPRN &&
                        x.LearnRefNumber == appsAdditionalPaymentLearningDeliveryInfo.LearnRefNumber &&
                        x.AimSeqNumber == appsAdditionalPaymentLearningDeliveryInfo.AimSeqNumber);

                    var aecApprenticeshipPriceEpisodePeriodisedValuesInfo = appsAdditionalPaymentRulebaseInfo.AECApprenticeshipPriceEpisodePeriodisedValues.Where(x =>
                        x.UKPRN == appsAdditionalPaymentLearningDeliveryInfo.UKPRN &&
                        x.LearnRefNumber == appsAdditionalPaymentLearningDeliveryInfo.LearnRefNumber &&
                        x.AimSeqNumber == appsAdditionalPaymentLearningDeliveryInfo.AimSeqNumber).ToList();

                    appsAdditionalPaymentsModels.Add(new AppsAdditionalPaymentsModel()
                    {
                        LearnerReferenceNumber = paymentInfo.LearnerReferenceNumber,
                        UniqueLearnerNumber = paymentInfo.LearnerUln,
                        ProviderSpecifiedLearnerMonitoringA = learner.ProviderSpecLearnerMonitorings?.SingleOrDefault(psm =>
                            string.Equals(psm.ProvSpecLearnMonOccur, "A", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                        ProviderSpecifiedLearnerMonitoringB = learner.ProviderSpecLearnerMonitorings?.SingleOrDefault(psm =>
                            string.Equals(psm.ProvSpecLearnMonOccur, "B", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                        LearningStartDate = paymentInfo.LearningStartDate,
                        FundingLineType = paymentInfo.LearningAimFundingLineType,
                        EmployerNameFromApprenticeshipService = paymentInfo.EmployerName,
                        EmployerIdentifierFromILR = GetEmployerIdentifier(aecLearningDeliveryInfo, paymentInfo),
                        TypeOfAdditionalPayment = paymentInfo.TypeOfAdditionalPayment,
                        AugustEarnings = GetAugustEarnings(paymentInfo, aecApprenticeshipPriceEpisodePeriodisedValuesInfo)
                    });
                }
            }

            //appsAdditionalPaymentDasPaymentsInfo.Payments
            //    .GroupBy(p => new
            //    {
            //        p.UkPrn,
            //        p.LearnerReferenceNumber,
            //        p.LearnerUln,
            //        p.LearningStartDate,
            //        p.LearningAimFundingLineType,
            //        p.TypeOfAdditionalPayment,
            //        p.LearningAimReference,
            //        p.LearningAimProgrammeType,
            //        p.LearningAimStandardCode,
            //        p.LearningAimFrameworkCode,
            //        p.LearningAimPathwayCode
            //    });

            return new List<AppsAdditionalPaymentsModel>();
        }

        private decimal GetAugustEarnings(DASPaymentInfo paymentInfo, List<AECApprenticeshipPriceEpisodePeriodisedValuesInfo> aecApprenticeshipPriceEpisodePeriodisedValuesInfo)
        {
            decimal? result = 0;
            if (paymentInfo.TransactionType == 4 || paymentInfo.TransactionType == 6)
            {
                result = aecApprenticeshipPriceEpisodePeriodisedValuesInfo.SingleOrDefault(x => x.AttributeName.Equals("PriceEpisodeFirstEmp1618Pay"))?.Period_1.GetValueOrDefault()
                            +
                         aecApprenticeshipPriceEpisodePeriodisedValuesInfo.SingleOrDefault(x => x.AttributeName.Equals("PriceEpisodeSecondEmp1618Pay"))?.Period_1.GetValueOrDefault();
            }

            if (paymentInfo.TransactionType == 5 || paymentInfo.TransactionType == 7)
            {
                result = aecApprenticeshipPriceEpisodePeriodisedValuesInfo.SingleOrDefault(x => x.AttributeName.Equals("PriceEpisodeFirstProv1618Pay"))?.Period_1.GetValueOrDefault()
                         +
                         aecApprenticeshipPriceEpisodePeriodisedValuesInfo.SingleOrDefault(x => x.AttributeName.Equals("PriceEpisodeSecondProv1618Pay"))?.Period_1.GetValueOrDefault();
            }

            if (paymentInfo.TransactionType == 16)
            {
                result = aecApprenticeshipPriceEpisodePeriodisedValuesInfo.SingleOrDefault(x => x.AttributeName.Equals("PriceEpisodeLearnerAdditionalPayment"))?.Period_1.GetValueOrDefault();
            }

            return result.GetValueOrDefault();
        }

        private string GetEmployerIdentifier(AECLearningDeliveryInfo aecLearningDeliveryInfo, DASPaymentInfo paymentInfo)
        {
            var identifier = 0;
            if (paymentInfo.TransactionType == 4)
            {
                identifier = aecLearningDeliveryInfo.LearnDelEmpIdFirstAdditionalPaymentThreshold.GetValueOrDefault();
            }

            if (paymentInfo.TransactionType == 6)
            {
                identifier = aecLearningDeliveryInfo.LearnDelEmpIdSecondAdditionalPaymentThreshold.GetValueOrDefault();
            }

            return identifier == 0 ? "Not available" : identifier.ToString();
        }
    }
}
