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
        private const string AugustCollectionPeriod = "1819-R01";
        private const string SeptemberCollectionPeriod = "1819-R02";
        private const string OctoberCollectionPeriod = "1819-R03";
        private const string NovemberCollectionPeriod = "1819-R04";
        private const string DecemberCollectionPeriod = "1819-R05";
        private const string JanuaryCollectionPeriod = "1819-R06";
        private const string FebruaryCollectionPeriod = "1819-R07";
        private const string MarchCollectionPeriod = "1819-R08";
        private const string AprilCollectionPeriod = "1819-R09";
        private const string MayCollectionPeriod = "1819-R10";
        private const string JuneCollectionPeriod = "1819-R11";
        private const string JulyCollectionPeriod = "1819-R12";
        private const string R13CollectionPeriod = "1819-R13";
        private const string R14CollectionPeriod = "1819-R14";
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

        private int[] FundingSourceEmpty => new int[] { };

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

                    PopulateAugustPayments(model, paymentInfo);
                    PopulateSeptemberPayments(model, paymentInfo);
                    PopulateOctoberPayments(model, paymentInfo);
                    PopulateNovemberPayments(model, paymentInfo);
                    PopulateDecemberPayments(model, paymentInfo);
                    PopulateJanuaryPayments(model, paymentInfo);
                    PopulateFebruaryPayments(model, paymentInfo);
                    PopulateMarchPayments(model, paymentInfo);
                    PopulateAprilPayments(model, paymentInfo);
                    PopulateMayPayments(model, paymentInfo);
                    PopulateJunePayments(model, paymentInfo);
                    PopulateJulyPayments(model, paymentInfo);
                    PopulateR13Payments(model, paymentInfo);
                    PopulateR14Payments(model, paymentInfo);

                    PopulateAugustTotalPayments(model);
                    PopulateSeptemberTotalPayments(model);
                    PopulateOctoberTotalPayments(model);
                    PopulateNovemberTotalPayments(model);
                    PopulateDecemberTotalPayments(model);
                    PopulateJanuaryTotalPayments(model);
                    PopulateFebruaryTotalPayments(model);
                    PopulateMarchTotalPayments(model);
                    PopulateAprilTotalPayments(model);
                    PopulateMayTotalPayments(model);
                    PopulateJuneTotalPayments(model);
                    PopulateJulyTotalPayments(model);
                    PopulateR13TotalPayments(model);
                    PopulateR14TotalPayments(model);

                    PopulateTotalLevyPayments(model);
                    PopulateTotalCoInvestmentPayments(model);
                    PopulateTotalCoInvestmentDueFromEmployerPayments(model);
                    PopulateTotalEmployerAdditionalPayments(model);
                    PopulateTotalProviderAdditionalPayments(model);
                    PopulateTotalApprenticeAdditionalPayments(model);
                    PopulateTotalEnglishAndMathsPayments(model);
                    PopulateTotalPaymentsForLearningSupport(model);

                    PopulateTotalPayments(model);
                    appsMonthlyPaymentModels.Add(model);
                }
            }

            return appsMonthlyPaymentModels;
        }

        #region PopulateMonthlyPayments

        private void PopulateAugustPayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.AugustLevyPayments = GetPayments(paymentInfo, AugustCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.AugustCoInvestmentPayments = GetPayments(paymentInfo, AugustCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.AugustCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, AugustCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.AugustEmployerAdditionalPayments = GetPayments(paymentInfo, AugustCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.AugustProviderAdditionalPayments = GetPayments(paymentInfo, AugustCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.AugustApprenticeAdditionalPayments = GetPayments(paymentInfo, AugustCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.AugustEnglishAndMathsPayments = GetPayments(paymentInfo, AugustCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.AugustPaymentsForLearningSupport = GetPayments(paymentInfo, AugustCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateSeptemberPayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.SeptemberLevyPayments = GetPayments(paymentInfo, SeptemberCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.SeptemberCoInvestmentPayments = GetPayments(paymentInfo, SeptemberCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.SeptemberCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, SeptemberCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.SeptemberEmployerAdditionalPayments = GetPayments(paymentInfo, SeptemberCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.SeptemberProviderAdditionalPayments = GetPayments(paymentInfo, SeptemberCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.SeptemberApprenticeAdditionalPayments = GetPayments(paymentInfo, SeptemberCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.SeptemberEnglishAndMathsPayments = GetPayments(paymentInfo, SeptemberCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.SeptemberPaymentsForLearningSupport = GetPayments(paymentInfo, SeptemberCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateOctoberPayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.OctoberLevyPayments = GetPayments(paymentInfo, OctoberCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.OctoberCoInvestmentPayments = GetPayments(paymentInfo, OctoberCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.OctoberCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, OctoberCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.OctoberEmployerAdditionalPayments = GetPayments(paymentInfo, OctoberCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.OctoberProviderAdditionalPayments = GetPayments(paymentInfo, OctoberCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.OctoberApprenticeAdditionalPayments = GetPayments(paymentInfo, OctoberCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.OctoberEnglishAndMathsPayments = GetPayments(paymentInfo, OctoberCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.OctoberPaymentsForLearningSupport = GetPayments(paymentInfo, OctoberCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateNovemberPayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.NovemberLevyPayments = GetPayments(paymentInfo, NovemberCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.NovemberCoInvestmentPayments = GetPayments(paymentInfo, NovemberCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.NovemberCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, NovemberCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.NovemberEmployerAdditionalPayments = GetPayments(paymentInfo, NovemberCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.NovemberProviderAdditionalPayments = GetPayments(paymentInfo, NovemberCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.NovemberApprenticeAdditionalPayments = GetPayments(paymentInfo, NovemberCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.NovemberEnglishAndMathsPayments = GetPayments(paymentInfo, NovemberCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.NovemberPaymentsForLearningSupport = GetPayments(paymentInfo, NovemberCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateDecemberPayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.DecemberLevyPayments = GetPayments(paymentInfo, DecemberCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.DecemberCoInvestmentPayments = GetPayments(paymentInfo, DecemberCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.DecemberCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, DecemberCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.DecemberEmployerAdditionalPayments = GetPayments(paymentInfo, DecemberCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.DecemberProviderAdditionalPayments = GetPayments(paymentInfo, DecemberCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.DecemberApprenticeAdditionalPayments = GetPayments(paymentInfo, DecemberCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.DecemberEnglishAndMathsPayments = GetPayments(paymentInfo, DecemberCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.DecemberPaymentsForLearningSupport = GetPayments(paymentInfo, DecemberCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateJanuaryPayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.JanuaryLevyPayments = GetPayments(paymentInfo, JanuaryCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.JanuaryCoInvestmentPayments = GetPayments(paymentInfo, JanuaryCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.JanuaryCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, JanuaryCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.JanuaryEmployerAdditionalPayments = GetPayments(paymentInfo, JanuaryCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.JanuaryProviderAdditionalPayments = GetPayments(paymentInfo, JanuaryCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.JanuaryApprenticeAdditionalPayments = GetPayments(paymentInfo, JanuaryCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.JanuaryEnglishAndMathsPayments = GetPayments(paymentInfo, JanuaryCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.JanuaryPaymentsForLearningSupport = GetPayments(paymentInfo, JanuaryCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateFebruaryPayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.FebruaryLevyPayments = GetPayments(paymentInfo, FebruaryCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.FebruaryCoInvestmentPayments = GetPayments(paymentInfo, FebruaryCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.FebruaryCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, FebruaryCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.FebruaryEmployerAdditionalPayments = GetPayments(paymentInfo, FebruaryCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.FebruaryProviderAdditionalPayments = GetPayments(paymentInfo, FebruaryCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.FebruaryApprenticeAdditionalPayments = GetPayments(paymentInfo, FebruaryCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.FebruaryEnglishAndMathsPayments = GetPayments(paymentInfo, FebruaryCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.FebruaryPaymentsForLearningSupport = GetPayments(paymentInfo, FebruaryCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateMarchPayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.MarchLevyPayments = GetPayments(paymentInfo, MarchCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.MarchCoInvestmentPayments = GetPayments(paymentInfo, MarchCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.MarchCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, MarchCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.MarchEmployerAdditionalPayments = GetPayments(paymentInfo, MarchCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.MarchProviderAdditionalPayments = GetPayments(paymentInfo, MarchCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.MarchApprenticeAdditionalPayments = GetPayments(paymentInfo, MarchCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.MarchEnglishAndMathsPayments = GetPayments(paymentInfo, MarchCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.MarchPaymentsForLearningSupport = GetPayments(paymentInfo, MarchCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateAprilPayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.AprilLevyPayments = GetPayments(paymentInfo, AprilCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.AprilCoInvestmentPayments = GetPayments(paymentInfo, AprilCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.AprilCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, AprilCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.AprilEmployerAdditionalPayments = GetPayments(paymentInfo, AprilCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.AprilProviderAdditionalPayments = GetPayments(paymentInfo, AprilCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.AprilApprenticeAdditionalPayments = GetPayments(paymentInfo, AprilCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.AprilEnglishAndMathsPayments = GetPayments(paymentInfo, AprilCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.AprilPaymentsForLearningSupport = GetPayments(paymentInfo, AprilCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateMayPayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.MayLevyPayments = GetPayments(paymentInfo, MayCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.MayCoInvestmentPayments = GetPayments(paymentInfo, MayCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.MayCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, MayCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.MayEmployerAdditionalPayments = GetPayments(paymentInfo, MayCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.MayProviderAdditionalPayments = GetPayments(paymentInfo, MayCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.MayApprenticeAdditionalPayments = GetPayments(paymentInfo, MayCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.MayEnglishAndMathsPayments = GetPayments(paymentInfo, MayCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.MayPaymentsForLearningSupport = GetPayments(paymentInfo, MayCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateJunePayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.JuneLevyPayments = GetPayments(paymentInfo, JuneCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.JuneCoInvestmentPayments = GetPayments(paymentInfo, JuneCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.JuneCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, JuneCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.JuneEmployerAdditionalPayments = GetPayments(paymentInfo, JuneCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.JuneProviderAdditionalPayments = GetPayments(paymentInfo, JuneCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.JuneApprenticeAdditionalPayments = GetPayments(paymentInfo, JuneCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.JuneEnglishAndMathsPayments = GetPayments(paymentInfo, JuneCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.JunePaymentsForLearningSupport = GetPayments(paymentInfo, JuneCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateJulyPayments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.JulyLevyPayments = GetPayments(paymentInfo, JulyCollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.JulyCoInvestmentPayments = GetPayments(paymentInfo, JulyCollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.JulyCoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, JulyCollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.JulyEmployerAdditionalPayments = GetPayments(paymentInfo, JulyCollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.JulyProviderAdditionalPayments = GetPayments(paymentInfo, JulyCollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.JulyApprenticeAdditionalPayments = GetPayments(paymentInfo, JulyCollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.JulyEnglishAndMathsPayments = GetPayments(paymentInfo, JulyCollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.JulyPaymentsForLearningSupport = GetPayments(paymentInfo, JulyCollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateR13Payments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.R13LevyPayments = GetPayments(paymentInfo, R13CollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.R13CoInvestmentPayments = GetPayments(paymentInfo, R13CollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.R13CoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, R13CollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.R13EmployerAdditionalPayments = GetPayments(paymentInfo, R13CollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.R13ProviderAdditionalPayments = GetPayments(paymentInfo, R13CollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.R13ApprenticeAdditionalPayments = GetPayments(paymentInfo, R13CollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.R13EnglishAndMathsPayments = GetPayments(paymentInfo, R13CollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.R13PaymentsForLearningSupport = GetPayments(paymentInfo, R13CollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        private void PopulateR14Payments(AppsMonthlyPaymentModel model, AppsMonthlyPaymentDASPaymentInfo paymentInfo)
        {
            model.R14LevyPayments = GetPayments(paymentInfo, R14CollectionPeriod, _fundingSourceLevyPayments, _transactionTypesLevyPayments);
            model.R14CoInvestmentPayments = GetPayments(paymentInfo, R14CollectionPeriod, _fundingSourceCoInvestmentPayments, _transactionTypesCoInvestmentPayments);
            model.R14CoInvestmentDueFromEmployerPayments = GetPayments(paymentInfo, R14CollectionPeriod, _fundingSourceCoInvestmentDueFromEmployer, _transactionTypesCoInvestmentFromEmployer);
            model.R14EmployerAdditionalPayments = GetPayments(paymentInfo, R14CollectionPeriod, FundingSourceEmpty, _transactionTypesEmployerAdditionalPayments);
            model.R14ProviderAdditionalPayments = GetPayments(paymentInfo, R14CollectionPeriod, FundingSourceEmpty, _transactionTypesProviderAdditionalPayments);
            model.R14ApprenticeAdditionalPayments = GetPayments(paymentInfo, R14CollectionPeriod, FundingSourceEmpty, _transactionTypesApprenticeshipAdditionalPayments);
            model.R14EnglishAndMathsPayments = GetPayments(paymentInfo, R14CollectionPeriod, FundingSourceEmpty, _transactionTypesEnglishAndMathsPayments);
            model.R14PaymentsForLearningSupport = GetPayments(paymentInfo, R14CollectionPeriod, FundingSourceEmpty, _transactionTypesLearningSupportPayments);
        }

        #endregion

        #region Populate Monthly Total Payments
        private void PopulateAugustTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.AugustTotalPayments = model.AugustLevyPayments +
                                        model.AugustCoInvestmentPayments +
                                        model.AugustEmployerAdditionalPayments +
                                        model.AugustProviderAdditionalPayments +
                                        model.AugustApprenticeAdditionalPayments +
                                        model.AugustEnglishAndMathsPayments +
                                        model.AugustPaymentsForLearningSupport;
        }

        private void PopulateSeptemberTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.SeptemberTotalPayments = model.SeptemberLevyPayments +
                                           model.SeptemberCoInvestmentPayments +
                                           model.SeptemberEmployerAdditionalPayments +
                                           model.SeptemberProviderAdditionalPayments +
                                           model.SeptemberApprenticeAdditionalPayments +
                                           model.SeptemberEnglishAndMathsPayments +
                                           model.SeptemberPaymentsForLearningSupport;
        }

        private void PopulateOctoberTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.OctoberTotalPayments = model.OctoberLevyPayments +
                                         model.OctoberCoInvestmentPayments +
                                         model.OctoberEmployerAdditionalPayments +
                                         model.OctoberProviderAdditionalPayments +
                                         model.OctoberApprenticeAdditionalPayments +
                                         model.OctoberEnglishAndMathsPayments +
                                         model.OctoberPaymentsForLearningSupport;
        }

        private void PopulateNovemberTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.NovemberTotalPayments = model.NovemberLevyPayments +
                                          model.NovemberCoInvestmentPayments +
                                          model.NovemberEmployerAdditionalPayments +
                                          model.NovemberProviderAdditionalPayments +
                                          model.NovemberApprenticeAdditionalPayments +
                                          model.NovemberEnglishAndMathsPayments +
                                          model.NovemberPaymentsForLearningSupport;
        }

        private void PopulateDecemberTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.DecemberTotalPayments = model.DecemberLevyPayments +
                                          model.DecemberCoInvestmentPayments +
                                          model.DecemberEmployerAdditionalPayments +
                                          model.DecemberProviderAdditionalPayments +
                                          model.DecemberApprenticeAdditionalPayments +
                                          model.DecemberEnglishAndMathsPayments +
                                          model.DecemberPaymentsForLearningSupport;
        }

        private void PopulateJanuaryTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.JanuaryTotalPayments = model.JanuaryLevyPayments +
                                         model.JanuaryCoInvestmentPayments +
                                         model.JanuaryEmployerAdditionalPayments +
                                         model.JanuaryProviderAdditionalPayments +
                                         model.JanuaryApprenticeAdditionalPayments +
                                         model.JanuaryEnglishAndMathsPayments +
                                         model.JanuaryPaymentsForLearningSupport;
        }

        private void PopulateFebruaryTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.FebruaryTotalPayments = model.FebruaryLevyPayments +
                                          model.FebruaryCoInvestmentPayments +
                                          model.FebruaryEmployerAdditionalPayments +
                                          model.FebruaryProviderAdditionalPayments +
                                          model.FebruaryApprenticeAdditionalPayments +
                                          model.FebruaryEnglishAndMathsPayments +
                                          model.FebruaryPaymentsForLearningSupport;
        }

        private void PopulateMarchTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.MarchTotalPayments = model.MarchLevyPayments +
                                       model.MarchCoInvestmentPayments +
                                       model.MarchEmployerAdditionalPayments +
                                       model.MarchProviderAdditionalPayments +
                                       model.MarchApprenticeAdditionalPayments +
                                       model.MarchEnglishAndMathsPayments +
                                       model.MarchPaymentsForLearningSupport;
        }

        private void PopulateAprilTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.AprilTotalPayments = model.AprilLevyPayments +
                                       model.AprilCoInvestmentPayments +
                                       model.AprilEmployerAdditionalPayments +
                                       model.AprilProviderAdditionalPayments +
                                       model.AprilApprenticeAdditionalPayments +
                                       model.AprilEnglishAndMathsPayments +
                                       model.AprilPaymentsForLearningSupport;
        }

        private void PopulateMayTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.MayTotalPayments = model.MayLevyPayments +
                                     model.MayCoInvestmentPayments +
                                     model.MayEmployerAdditionalPayments +
                                     model.MayProviderAdditionalPayments +
                                     model.MayApprenticeAdditionalPayments +
                                     model.MayEnglishAndMathsPayments +
                                     model.MayPaymentsForLearningSupport;
        }

        private void PopulateJuneTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.JuneTotalPayments = model.JuneLevyPayments +
                                      model.JuneCoInvestmentPayments +
                                      model.JuneEmployerAdditionalPayments +
                                      model.JuneProviderAdditionalPayments +
                                      model.JuneApprenticeAdditionalPayments +
                                      model.JuneEnglishAndMathsPayments +
                                      model.JunePaymentsForLearningSupport;
        }

        private void PopulateJulyTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.JulyTotalPayments = model.JulyLevyPayments +
                                      model.JulyCoInvestmentPayments +
                                      model.JulyEmployerAdditionalPayments +
                                      model.JulyProviderAdditionalPayments +
                                      model.JulyApprenticeAdditionalPayments +
                                      model.JulyEnglishAndMathsPayments +
                                      model.JulyPaymentsForLearningSupport;
        }

        private void PopulateR13TotalPayments(AppsMonthlyPaymentModel model)
        {
            model.R13TotalPayments = model.R13LevyPayments +
                                     model.R13CoInvestmentPayments +
                                     model.R13EmployerAdditionalPayments +
                                     model.R13ProviderAdditionalPayments +
                                     model.R13ApprenticeAdditionalPayments +
                                     model.R13EnglishAndMathsPayments +
                                     model.R13PaymentsForLearningSupport;
        }

        private void PopulateR14TotalPayments(AppsMonthlyPaymentModel model)
        {
            model.R14TotalPayments = model.R14LevyPayments +
                                     model.R14CoInvestmentPayments +
                                     model.R14EmployerAdditionalPayments +
                                     model.R14ProviderAdditionalPayments +
                                     model.R14ApprenticeAdditionalPayments +
                                     model.R14EnglishAndMathsPayments +
                                     model.R14PaymentsForLearningSupport;
        }

        private void PopulateTotalLevyPayments(AppsMonthlyPaymentModel model)
        {
            model.TotalLevyPayments = model.AugustLevyPayments + model.SeptemberLevyPayments +
                                      model.OctoberLevyPayments + model.NovemberLevyPayments +
                                      model.DecemberLevyPayments + model.JanuaryLevyPayments +
                                      model.FebruaryLevyPayments + model.MarchLevyPayments +
                                      model.AprilLevyPayments + model.MayLevyPayments + model.JuneLevyPayments +
                                      model.JulyLevyPayments +
                                      model.R13LevyPayments + model.R14LevyPayments;
        }

        private void PopulateTotalCoInvestmentPayments(AppsMonthlyPaymentModel model)
        {
            model.TotalCoInvestmentPayments = model.AugustCoInvestmentPayments + model.SeptemberCoInvestmentPayments +
                                      model.OctoberCoInvestmentPayments + model.NovemberCoInvestmentPayments +
                                      model.DecemberCoInvestmentPayments + model.JanuaryCoInvestmentPayments +
                                      model.FebruaryCoInvestmentPayments + model.MarchCoInvestmentPayments +
                                      model.AprilCoInvestmentPayments + model.MayCoInvestmentPayments + model.JuneCoInvestmentPayments +
                                      model.JulyCoInvestmentPayments +
                                      model.R13CoInvestmentPayments + model.R14CoInvestmentPayments;
        }

        private void PopulateTotalCoInvestmentDueFromEmployerPayments(AppsMonthlyPaymentModel model)
        {
            model.TotalCoInvestmentDueFromEmployerPayments = model.AugustCoInvestmentDueFromEmployerPayments + model.SeptemberApprenticeAdditionalPayments +
                                      model.OctoberCoInvestmentDueFromEmployerPayments + model.NovemberCoInvestmentDueFromEmployerPayments +
                                      model.DecemberCoInvestmentDueFromEmployerPayments + model.JanuaryCoInvestmentDueFromEmployerPayments +
                                      model.FebruaryCoInvestmentDueFromEmployerPayments + model.MarchCoInvestmentDueFromEmployerPayments +
                                      model.AprilCoInvestmentDueFromEmployerPayments + model.MayCoInvestmentDueFromEmployerPayments + model.JuneCoInvestmentDueFromEmployerPayments +
                                      model.JulyCoInvestmentDueFromEmployerPayments +
                                      model.R13CoInvestmentDueFromEmployerPayments + model.R14CoInvestmentDueFromEmployerPayments;
        }

        private void PopulateTotalEmployerAdditionalPayments(AppsMonthlyPaymentModel model)
        {
            model.TotalEmployerAdditionalPayments = model.AugustEmployerAdditionalPayments + model.SeptemberEmployerAdditionalPayments +
                                      model.OctoberEmployerAdditionalPayments + model.NovemberEmployerAdditionalPayments +
                                      model.DecemberEmployerAdditionalPayments + model.JanuaryEmployerAdditionalPayments +
                                      model.FebruaryEmployerAdditionalPayments + model.MarchEmployerAdditionalPayments +
                                      model.AprilEmployerAdditionalPayments + model.MayEmployerAdditionalPayments + model.JuneEmployerAdditionalPayments +
                                      model.JulyEmployerAdditionalPayments +
                                      model.R13EmployerAdditionalPayments + model.R14EmployerAdditionalPayments;
        }

        private void PopulateTotalProviderAdditionalPayments(AppsMonthlyPaymentModel model)
        {
            model.TotalProviderAdditionalPayments = model.AugustProviderAdditionalPayments + model.SeptemberProviderAdditionalPayments +
                                                    model.OctoberProviderAdditionalPayments + model.NovemberProviderAdditionalPayments +
                                                    model.DecemberProviderAdditionalPayments + model.JanuaryProviderAdditionalPayments +
                                                    model.FebruaryProviderAdditionalPayments + model.MarchProviderAdditionalPayments +
                                                    model.AprilProviderAdditionalPayments + model.MayProviderAdditionalPayments + model.JuneProviderAdditionalPayments +
                                                    model.JulyProviderAdditionalPayments +
                                                    model.R13ProviderAdditionalPayments + model.R14ProviderAdditionalPayments;
        }

        private void PopulateTotalApprenticeAdditionalPayments(AppsMonthlyPaymentModel model)
        {
            model.TotalApprenticeAdditionalPayments = model.AugustApprenticeAdditionalPayments + model.SeptemberApprenticeAdditionalPayments +
                                                      model.OctoberApprenticeAdditionalPayments + model.NovemberApprenticeAdditionalPayments +
                                                      model.DecemberApprenticeAdditionalPayments + model.JanuaryApprenticeAdditionalPayments +
                                                      model.FebruaryApprenticeAdditionalPayments + model.MarchApprenticeAdditionalPayments +
                                                      model.AprilApprenticeAdditionalPayments + model.MayApprenticeAdditionalPayments + model.JuneApprenticeAdditionalPayments +
                                                      model.JulyApprenticeAdditionalPayments +
                                                      model.R13ApprenticeAdditionalPayments + model.R14ApprenticeAdditionalPayments;
        }

        private void PopulateTotalEnglishAndMathsPayments(AppsMonthlyPaymentModel model)
        {
            model.TotalEnglishAndMathsPayments = model.AugustEnglishAndMathsPayments + model.SeptemberEnglishAndMathsPayments +
                                                 model.OctoberEnglishAndMathsPayments + model.NovemberEnglishAndMathsPayments +
                                                 model.DecemberEnglishAndMathsPayments + model.JanuaryEnglishAndMathsPayments +
                                                 model.FebruaryEnglishAndMathsPayments + model.MarchEnglishAndMathsPayments +
                                                 model.AprilEnglishAndMathsPayments + model.MayEnglishAndMathsPayments + model.JuneEnglishAndMathsPayments +
                                                 model.JulyEnglishAndMathsPayments +
                                                 model.R13EnglishAndMathsPayments + model.R14EnglishAndMathsPayments;
        }

        private void PopulateTotalPaymentsForLearningSupport(AppsMonthlyPaymentModel model)
        {
            model.TotalPaymentsForLearningSupport = model.AugustPaymentsForLearningSupport + model.SeptemberPaymentsForLearningSupport +
                                                    model.OctoberPaymentsForLearningSupport + model.NovemberPaymentsForLearningSupport +
                                                    model.DecemberPaymentsForLearningSupport + model.JanuaryPaymentsForLearningSupport +
                                                    model.FebruaryPaymentsForLearningSupport + model.MarchPaymentsForLearningSupport +
                                                    model.AprilPaymentsForLearningSupport + model.MayPaymentsForLearningSupport + model.JunePaymentsForLearningSupport +
                                                    model.JulyPaymentsForLearningSupport +
                                                    model.R13PaymentsForLearningSupport + model.R14PaymentsForLearningSupport;
        }

        private void PopulateTotalPayments(AppsMonthlyPaymentModel model)
        {
            model.TotalPayments = model.TotalLevyPayments +
                                  model.TotalCoInvestmentPayments +
                                  model.TotalEmployerAdditionalPayments +
                                  model.TotalProviderAdditionalPayments +
                                  model.TotalApprenticeAdditionalPayments +
                                  model.TotalEnglishAndMathsPayments +
                                  model.TotalPaymentsForLearningSupport;
        }
        #endregion

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