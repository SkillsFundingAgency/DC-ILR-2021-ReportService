using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Interface.Builders.PeriodEnd;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsMonthlyPayment;
using ESFA.DC.ILR.ReportService.Model.ReportModels.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Extensions;

namespace ESFA.DC.ILR.ReportService.Service.Builders.PeriodEnd
{
    public class AppsMonthlyPaymentModelBuilder : IAppsMonthlyPaymentModelBuilder
    {
       private readonly string[] _collectionPeriods = {
            "1819-R01",
            "1819-R02",
            "1819-R03",
            "1819-R04",
            "1819-R05",
            "1819-R06",
            "1819-R07",
            "1819-R08",
            "1819-R09",
            "1819-R10",
            "1819-R11",
            "1819-R12",
            "1819-R13",
            "1819-R14"
        };

        private readonly int[] _fundingSourceLevyPayments = { 1, 5 };
        private readonly int[] _fundingSourceCoInvestmentPayments = { 2 };
        private readonly int[] _fundingSourceCoInvestmentDueFromEmployer = { 3 };
        private readonly int[] _transactionTypesLevyPayments = { 1, 2, 3 };
        private readonly int[] _transactionTypesCoInvestmentPayments = { 1, 2, 3 };
        private readonly int[] _transactionTypesCoInvestmentFromEmployer = { 1, 2, 3 };
        private readonly int[] _transactionTypesEmployerAdditionalPayments = { 4, 6 };
        private readonly int[] _transactionTypesProviderAdditionalPayments = { 5, 7 };
        private readonly int[] _transactionTypesApprenticeshipAdditionalPayments = { 16 };
        private readonly int[] _transactionTypesEnglishAndMathsPayments = { 13, 14 };
        private readonly int[] _transactionTypesLearningSupportPayments = { 8, 9, 10, 11, 12, 15 };

        private int[] _fundingSourceEmpty => new int[] { };

        public List<AppsMonthlyPaymentModel> BuildModel(
            AppsMonthlyPaymentILRInfo appsMonthlyPaymentIlrInfo,
            AppsMonthlyPaymentRulebaseInfo appsMonthlyPaymentRulebaseInfo,
            AppsMonthlyPaymentDASInfo appsMonthlyPaymentDasInfo)
        {
            List<AppsMonthlyPaymentModel> appsMonthlyPaymentModels = new List<AppsMonthlyPaymentModel>();
            foreach (var learner in appsMonthlyPaymentIlrInfo.Learners)
            {
                foreach (var paymentInfo in appsMonthlyPaymentDasInfo.Payments)
                {
                    var learningDeliveryInfo = learner.LearningDeliveries.SingleOrDefault(x =>
                        x.UKPRN == paymentInfo.UkPrn &&
                        x.LearnRefNumber ==
                        paymentInfo.LearnerReferenceNumber &&
                        x.LearnAimRef == paymentInfo.LearningAimReference &&
                        x.LearnStartDate == paymentInfo.LearningStartDate &&
                        x.ProgType == paymentInfo.LearningAimProgrammeType &&
                        x.StdCode == paymentInfo.LearningAimStandardCode &&
                        x.FworkCode == paymentInfo.LearningAimFrameworkCode &&
                        x.PwayCode == paymentInfo.LearningAimPathwayCode);
                    var aecApprenticeshipPriceEpisode =
                        appsMonthlyPaymentRulebaseInfo.AECApprenticeshipPriceEpisodes.SingleOrDefault(x =>
                            x.UkPrn == learningDeliveryInfo?.UKPRN &&
                            x.LearnRefNumber == learningDeliveryInfo.LearnRefNumber &&
                            x.AimSequenceNumber == learningDeliveryInfo.AimSeqNumber);

                    var model = new AppsMonthlyPaymentModel()
                    {
                        LearnerReferenceNumber = paymentInfo.LearnerReferenceNumber,
                        UniqueLearnerNumber = paymentInfo.LearnerUln,
                        CampusIdentifier = learner.CampId,
                        ProviderSpecifiedLearnerMonitoringA = learner.ProviderSpecLearnerMonitorings
                            ?.SingleOrDefault(x =>
                                string.Equals(x.ProvSpecLearnMonOccur, "A", StringComparison.OrdinalIgnoreCase))
                            ?.ProvSpecLearnMon,
                        ProviderSpecifiedLearnerMonitoringB = learner.ProviderSpecLearnerMonitorings
                            ?.SingleOrDefault(x =>
                                string.Equals(x.ProvSpecLearnMonOccur, "B", StringComparison.OrdinalIgnoreCase))
                            ?.ProvSpecLearnMon,
                        AimSeqNumber = learningDeliveryInfo.AimSeqNumber,
                        LearningAimReference = paymentInfo.LearningAimReference,
                        LearningAimTitle = " ", // Todo: Get it from LarsProvider
                        LearningStartDate = paymentInfo.LearningStartDate.ToString("dd/MM/yyyy"),
                        LearningAimProgrammeType = paymentInfo.LearningAimProgrammeType,
                        LearningAimStandardCode = paymentInfo.LearningAimStandardCode,
                        LearningAimFrameworkCode = paymentInfo.LearningAimFrameworkCode,
                        LearningAimPathwayCode = paymentInfo.LearningAimPathwayCode,
                        AimType = learningDeliveryInfo.AimType,
                        SoftwareSupplierAimIdentifier = learningDeliveryInfo.SWSupAimId,
                        ProviderSpecifiedDeliveryMonitoringA = learningDeliveryInfo.ProviderSpecDeliveryMonitorings
                            ?.SingleOrDefault(x =>
                                string.Equals(x.ProvSpecDelMonOccur, "A", StringComparison.OrdinalIgnoreCase))
                            ?.ProvSpecDelMon,
                        ProviderSpecifiedDeliveryMonitoringB = learningDeliveryInfo.ProviderSpecDeliveryMonitorings
                            ?.SingleOrDefault(x =>
                                string.Equals(x.ProvSpecDelMonOccur, "B", StringComparison.OrdinalIgnoreCase))
                            ?.ProvSpecDelMon,
                        ProviderSpecifiedDeliveryMonitoringC = learningDeliveryInfo.ProviderSpecDeliveryMonitorings
                            ?.SingleOrDefault(x =>
                                string.Equals(x.ProvSpecDelMonOccur, "C", StringComparison.OrdinalIgnoreCase))
                            ?.ProvSpecDelMon,
                        ProviderSpecifiedDeliveryMonitoringD = learningDeliveryInfo.ProviderSpecDeliveryMonitorings
                            ?.SingleOrDefault(x =>
                                string.Equals(x.ProvSpecDelMonOccur, "D", StringComparison.OrdinalIgnoreCase))
                            ?.ProvSpecDelMon,
                        EndPointAssessmentOrganisation = learningDeliveryInfo.EPAOrganisation,
                        SubContractedOrPartnershipUKPRN = learningDeliveryInfo.PartnerUkPrn,
                        PriceEpisodeStartDate = paymentInfo.LearningAimReference.Equals("ZPROG001")
                            ? paymentInfo.PriceEpisodeIdentifier.Substring(
                                paymentInfo.PriceEpisodeIdentifier.Length - 10)
                            : string.Empty,
                        PriceEpisodeActualEndDate = aecApprenticeshipPriceEpisode?.PriceEpisodeActualEndDate
                            .GetValueOrDefault().ToString("dd/MM/yyyy"),
                        FundingLineType = paymentInfo.LearningAimFundingLineType,
                        LearningDeliveryFAMTypeApprenticeshipContractType = paymentInfo.ContractType,
                        AgreementIdentifier = aecApprenticeshipPriceEpisode?.PriceEpisodeAgreeId,
                    };

                    PopulatePayments(model, paymentInfo);
                    PopulateMonthlyTotalPayments(model);
                    model.TotalLevyPayments = model.LevyPayments.Sum();
                    model.TotalCoInvestmentPayments = model.CoInvestmentPayments.Sum();
                    model.TotalCoInvestmentDueFromEmployerPayments = model.CoInvestmentDueFromEmployerPayments.Sum();
                    model.TotalEmployerAdditionalPayments = model.EmployerAdditionalPayments.Sum();
                    model.TotalProviderAdditionalPayments = model.ProviderAdditionalPayments.Sum();
                    model.TotalApprenticeAdditionalPayments = model.ApprenticeAdditionalPayments.Sum();
                    model.TotalEnglishAndMathsPayments = model.EnglishAndMathsPayments.Sum();
                    model.TotalPaymentsForLearningSupport = model.PaymentsForLearningSupport.Sum();
                    PopulateTotalPayments(model);
                    appsMonthlyPaymentModels.Add(model);
                }
            }

            return appsMonthlyPaymentModels;
        }

        private void PopulatePayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.LevyPayments = new decimal[14];
            model.CoInvestmentPayments = new decimal[14];
            model.CoInvestmentDueFromEmployerPayments = new decimal[14];
            model.EmployerAdditionalPayments = new decimal[14];
            model.ProviderAdditionalPayments = new decimal[14];
            model.ApprenticeAdditionalPayments = new decimal[14];
            model.EnglishAndMathsPayments = new decimal[14];
            model.PaymentsForLearningSupport = new decimal[14];
            for (int i = 0; i <= 13; i++)
            {
                model.LevyPayments[i] = GetPayments(paymentInfo, _collectionPeriods[i], _fundingSourceLevyPayments, _transactionTypesLevyPayments);
                model.CoInvestmentPayments[i] = GetPayments(paymentInfo, _collectionPeriods[i], _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
                model.CoInvestmentDueFromEmployerPayments[i] = GetPayments(paymentInfo, _collectionPeriods[i], _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
                model.EmployerAdditionalPayments[i] = GetPayments(paymentInfo, _collectionPeriods[i], _fundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
                model.ProviderAdditionalPayments[i] = GetPayments(paymentInfo, _collectionPeriods[i], _fundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
                model.ApprenticeAdditionalPayments[i] = GetPayments(paymentInfo, _collectionPeriods[i], _fundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
                model.EnglishAndMathsPayments[i] = GetPayments(paymentInfo, _collectionPeriods[i], _fundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
                model.PaymentsForLearningSupport[i] = GetPayments(paymentInfo, _collectionPeriods[i], _fundingSourceEmpty, _transactionTypesLearningSupportPayments);
            }
        }

        private void PopulateMonthlyTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.TotalMonthlyPayments = new decimal[14];
            for (int i = 0; i <= 13; i++)
            {
                model.TotalMonthlyPayments[i] = model.LevyPayments[i] + model.CoInvestmentPayments[i] + model.CoInvestmentDueFromEmployerPayments[i] +
                                                model.EmployerAdditionalPayments[i] + model.ProviderAdditionalPayments[i] + model.ApprenticeAdditionalPayments[i] +
                                                model.EnglishAndMathsPayments[i] + model.PaymentsForLearningSupport[i];
            }
        }

        private void PopulateTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.TotalPayments = model.TotalLevyPayments +
                                  model.TotalCoInvestmentPayments +
                                  model.TotalCoInvestmentDueFromEmployerPayments +
                                  model.TotalEmployerAdditionalPayments +
                                  model.TotalProviderAdditionalPayments +
                                  model.TotalApprenticeAdditionalPayments +
                                  model.TotalEnglishAndMathsPayments +
                                  model.TotalPaymentsForLearningSupport;
        }

        private decimal GetPayments(AppsMonthlyPaymentDASPaymentInfo paymentInfo, string collectionPeriodName, int[] fundingSource, int[] transactionTypes)
        {
            decimal payment = 0;
            if (fundingSource.Length > 0)
            {
                if (paymentInfo.CollectionPeriod.ToCollectionPeriodName(paymentInfo.AcademicYear.ToString()).Equals(collectionPeriodName) &&
                    transactionTypes.Contains(paymentInfo.TransactionType) &&
                    fundingSource.Contains(paymentInfo.FundingSource))
                {
                    payment = paymentInfo.Amount;
                }
            }
            else if (paymentInfo.CollectionPeriod.ToCollectionPeriodName(paymentInfo.AcademicYear.ToString()).Equals(collectionPeriodName) &&
                     transactionTypes.Contains(paymentInfo.TransactionType))
            {
                payment = paymentInfo.Amount;
            }

            return payment;
        }
    }
}