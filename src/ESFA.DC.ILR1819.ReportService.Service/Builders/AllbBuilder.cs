using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public sealed class AllbBuilder : IAllbBuilder
    {
        private const string AlbSupportPayment = "ALBSupportPayment";
        private const string AlbAreaUpliftBalPayment = "AreaUpliftBalPayment";
        private const string AlbAreaUpliftOnProgPayment = "AreaUpliftOnProgPayment";

        private readonly IIlrProviderService _ilrProviderService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IAllbProviderService _allbProviderService;
        private readonly IPeriodProviderService _periodProviderService;
        private readonly ITotalBuilder _totalBuilder;
        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly ILogger _logger;

        public AllbBuilder(
            IIlrProviderService ilrProviderService,
            IValidLearnersService validLearnersService,
            IAllbProviderService allbProviderService,
            IPeriodProviderService periodProviderService,
            ITotalBuilder totalBuilder,
            IStringUtilitiesService stringUtilitiesService,
            ILogger logger)
        {
            _ilrProviderService = ilrProviderService;
            _validLearnersService = validLearnersService;
            _allbProviderService = allbProviderService;
            _periodProviderService = periodProviderService;
            _totalBuilder = totalBuilder;
            _stringUtilitiesService = stringUtilitiesService;
            _logger = logger;
        }

        public async Task<List<FundingSummaryModel>> BuildAsync(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            FundingSummaryModel fundingSummaryModelAlbFunding = new FundingSummaryModel()
            {
                Title = "ILR Advanced Loans Bursary Funding (£)"
            };

            FundingSummaryModel fundingSummaryModelAlbAreaCosts = new FundingSummaryModel()
            {
                Title = "ILR Advanced Loans Bursary Area Costs (£)"
            };

            List<FundingSummaryModel> fundingSummaryModels = new List<FundingSummaryModel>()
            {
                fundingSummaryModelAlbFunding,
                fundingSummaryModelAlbAreaCosts
            };

            Task<IMessage> ilrFile = _ilrProviderService.GetIlrFile(jobContextMessage, cancellationToken);
            Task<List<string>> validLearners = _validLearnersService.GetLearnersAsync(jobContextMessage, cancellationToken);
            Task<ALBGlobal> albData = _allbProviderService.GetAllbData(jobContextMessage, cancellationToken);
            Task<int> period = _periodProviderService.GetPeriod(jobContextMessage, cancellationToken);

            await Task.WhenAll(ilrFile, validLearners, albData, period);

            List<string> ilrError = new List<string>();
            List<string> albLearnerError = new List<string>();

            foreach (string validLearnerRefNum in validLearners.Result)
            {
                var learner = ilrFile?.Result.Learners?.SingleOrDefault(x => string.Equals(x.LearnRefNumber, validLearnerRefNum, StringComparison.OrdinalIgnoreCase));
                if (learner == null)
                {
                    ilrError.Add(validLearnerRefNum);
                    continue;
                }

                ALBLearner albLearner =
                    albData?.Result.Learners?.SingleOrDefault(x => x.LearnRefNumber == validLearnerRefNum);
                if (albLearner == null)
                {
                    albLearnerError.Add(validLearnerRefNum);
                    continue;
                }

                TotalAlb(albLearner, fundingSummaryModelAlbFunding, period.Result, fundingSummaryModelAlbAreaCosts);
            }

            if (ilrError.Any())
            {
                _logger.LogWarning($"Failed to get one or more ILR learners while {nameof(AllbBuilder)}.{nameof(BuildAsync)}: {_stringUtilitiesService.JoinWithMaxLength(ilrError)}");
            }

            if (albLearnerError.Any())
            {
                _logger.LogWarning($"Failed to get one or more ALB learners while {nameof(AllbBuilder)}.{nameof(BuildAsync)}: {_stringUtilitiesService.JoinWithMaxLength(albLearnerError)}");
            }

            return fundingSummaryModels;
        }

        private void TotalAlb(
            ALBLearner albLearner,
            FundingSummaryModel fundingSummaryModelAlbFunding,
            int period,
            FundingSummaryModel fundingSummaryModelAlbAreaCosts)
        {
            LearnerPeriodisedValue albSupportPaymentObj =
                albLearner.LearnerPeriodisedValues.SingleOrDefault(x => x.AttributeName == AlbSupportPayment);
            LearnerPeriodisedValue albAreaUpliftOnProgPaymentObj =
                albLearner.LearnerPeriodisedValues.SingleOrDefault(x => x.AttributeName == AlbAreaUpliftOnProgPayment);
            LearnerPeriodisedValue albAreaUpliftBalPaymentObj =
                albLearner.LearnerPeriodisedValues.SingleOrDefault(x => x.AttributeName == AlbAreaUpliftBalPayment);

            fundingSummaryModelAlbFunding.Period1 =
                fundingSummaryModelAlbFunding.Period1 + albSupportPaymentObj?.Period1 ?? 0;
            fundingSummaryModelAlbFunding.Period2 =
                fundingSummaryModelAlbFunding.Period2 + albSupportPaymentObj?.Period2 ?? 0;
            fundingSummaryModelAlbFunding.Period3 =
                fundingSummaryModelAlbFunding.Period3 + albSupportPaymentObj?.Period3 ?? 0;
            fundingSummaryModelAlbFunding.Period4 =
                fundingSummaryModelAlbFunding.Period1 + albSupportPaymentObj?.Period4 ?? 0;
            fundingSummaryModelAlbFunding.Period5 =
                fundingSummaryModelAlbFunding.Period5 + albSupportPaymentObj?.Period5 ?? 0;
            fundingSummaryModelAlbFunding.Period6 =
                fundingSummaryModelAlbFunding.Period6 + albSupportPaymentObj?.Period6 ?? 0;
            fundingSummaryModelAlbFunding.Period7 =
                fundingSummaryModelAlbFunding.Period7 + albSupportPaymentObj?.Period7 ?? 0;
            fundingSummaryModelAlbFunding.Period8 =
                fundingSummaryModelAlbFunding.Period8 + albSupportPaymentObj?.Period8 ?? 0;
            fundingSummaryModelAlbFunding.Period9 =
                fundingSummaryModelAlbFunding.Period9 + albSupportPaymentObj?.Period9 ?? 0;
            fundingSummaryModelAlbFunding.Period10 =
                fundingSummaryModelAlbFunding.Period10 + albSupportPaymentObj?.Period10 ?? 0;
            fundingSummaryModelAlbFunding.Period11 =
                fundingSummaryModelAlbFunding.Period11 + albSupportPaymentObj?.Period11 ?? 0;
            fundingSummaryModelAlbFunding.Period12 =
                fundingSummaryModelAlbFunding.Period12 + albSupportPaymentObj?.Period12 ?? 0;
            fundingSummaryModelAlbFunding.Period1_8 =
                _totalBuilder.TotalRecords(
                    fundingSummaryModelAlbFunding.Period1_8,
                    fundingSummaryModelAlbFunding.Period1,
                    fundingSummaryModelAlbFunding.Period2,
                    fundingSummaryModelAlbFunding.Period3,
                    fundingSummaryModelAlbFunding.Period4,
                    fundingSummaryModelAlbFunding.Period5,
                    fundingSummaryModelAlbFunding.Period6,
                    fundingSummaryModelAlbFunding.Period7,
                    fundingSummaryModelAlbFunding.Period8);
            fundingSummaryModelAlbFunding.Period9_12 =
                _totalBuilder.TotalRecords(
                    fundingSummaryModelAlbFunding.Period9_12,
                    fundingSummaryModelAlbFunding.Period9,
                    fundingSummaryModelAlbFunding.Period10,
                    fundingSummaryModelAlbFunding.Period11,
                    fundingSummaryModelAlbFunding.Period12);
            fundingSummaryModelAlbFunding.YearToDate = fundingSummaryModelAlbFunding.YearToDate +
                                                       GetYearToDateTotal(albSupportPaymentObj, period);
            fundingSummaryModelAlbFunding.Total = _totalBuilder.TotalRecords(
                fundingSummaryModelAlbFunding.Total,
                fundingSummaryModelAlbFunding.Period1_8,
                fundingSummaryModelAlbFunding.Period9_12);

            fundingSummaryModelAlbAreaCosts.Period1 = fundingSummaryModelAlbAreaCosts.Period1 +
                                                      (albAreaUpliftBalPaymentObj?.Period1 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period1 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period2 = fundingSummaryModelAlbAreaCosts.Period2 +
                                                      (albAreaUpliftBalPaymentObj?.Period2 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period2 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period3 = fundingSummaryModelAlbAreaCosts.Period3 +
                                                      (albAreaUpliftBalPaymentObj?.Period3 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period3 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period4 = fundingSummaryModelAlbAreaCosts.Period4 +
                                                      (albAreaUpliftBalPaymentObj?.Period4 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period4 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period5 = fundingSummaryModelAlbAreaCosts.Period5 +
                                                      (albAreaUpliftBalPaymentObj?.Period5 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period5 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period6 = fundingSummaryModelAlbAreaCosts.Period6 +
                                                      (albAreaUpliftBalPaymentObj?.Period6 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period6 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period7 = fundingSummaryModelAlbAreaCosts.Period7 +
                                                      (albAreaUpliftBalPaymentObj?.Period7 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period7 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period8 = fundingSummaryModelAlbAreaCosts.Period8 +
                                                      (albAreaUpliftBalPaymentObj?.Period8 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period8 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period9 = fundingSummaryModelAlbAreaCosts.Period9 +
                                                      (albAreaUpliftBalPaymentObj?.Period9 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period9 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period10 = fundingSummaryModelAlbAreaCosts.Period10 +
                                                       (albAreaUpliftBalPaymentObj?.Period10 ?? 0) +
                                                       (albAreaUpliftOnProgPaymentObj?.Period10 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period11 = fundingSummaryModelAlbAreaCosts.Period11 +
                                                       (albAreaUpliftBalPaymentObj?.Period11 ?? 0) +
                                                       (albAreaUpliftOnProgPaymentObj?.Period11 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period12 = fundingSummaryModelAlbAreaCosts.Period12 +
                                                       (albAreaUpliftBalPaymentObj?.Period12 ?? 0) +
                                                       (albAreaUpliftOnProgPaymentObj?.Period12 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period1_8 =
                _totalBuilder.TotalRecords(
                    fundingSummaryModelAlbAreaCosts.Period1_8,
                    fundingSummaryModelAlbAreaCosts.Period1,
                    fundingSummaryModelAlbAreaCosts.Period2,
                    fundingSummaryModelAlbAreaCosts.Period3,
                    fundingSummaryModelAlbAreaCosts.Period4,
                    fundingSummaryModelAlbAreaCosts.Period5,
                    fundingSummaryModelAlbAreaCosts.Period6,
                    fundingSummaryModelAlbAreaCosts.Period7,
                    fundingSummaryModelAlbAreaCosts.Period8);
            fundingSummaryModelAlbAreaCosts.Period9_12 =
                _totalBuilder.TotalRecords(
                    fundingSummaryModelAlbAreaCosts.Period9_12,
                    fundingSummaryModelAlbAreaCosts.Period9,
                    fundingSummaryModelAlbAreaCosts.Period10,
                    fundingSummaryModelAlbAreaCosts.Period11,
                    fundingSummaryModelAlbAreaCosts.Period12);
            fundingSummaryModelAlbAreaCosts.YearToDate = fundingSummaryModelAlbAreaCosts.YearToDate +
                                                         GetYearToDateTotal(albAreaUpliftBalPaymentObj, albAreaUpliftOnProgPaymentObj, period);
            fundingSummaryModelAlbAreaCosts.Total =
                _totalBuilder.TotalRecords(
                    fundingSummaryModelAlbAreaCosts.Total,
                    fundingSummaryModelAlbAreaCosts.Period1_8,
                    fundingSummaryModelAlbAreaCosts.Period9_12);
        }

        private decimal? GetYearToDateTotal(LearnerPeriodisedValue albSupportPaymentObj, int period)
        {
            decimal total = 0;
            for (int i = 0; i < period; i++)
            {
                switch (i)
                {
                    case 0:
                        total += albSupportPaymentObj?.Period1 ?? 0;
                        break;
                    case 1:
                        total += albSupportPaymentObj?.Period2 ?? 0;
                        break;
                    case 2:
                        total += albSupportPaymentObj?.Period3 ?? 0;
                        break;
                    case 3:
                        total += albSupportPaymentObj?.Period4 ?? 0;
                        break;
                    case 4:
                        total += albSupportPaymentObj?.Period5 ?? 0;
                        break;
                    case 5:
                        total += albSupportPaymentObj?.Period6 ?? 0;
                        break;
                    case 6:
                        total += albSupportPaymentObj?.Period7 ?? 0;
                        break;
                    case 7:
                        total += albSupportPaymentObj?.Period8 ?? 0;
                        break;
                    case 8:
                        total += albSupportPaymentObj?.Period9 ?? 0;
                        break;
                    case 9:
                        total += albSupportPaymentObj?.Period10 ?? 0;
                        break;
                    case 10:
                        total += albSupportPaymentObj?.Period11 ?? 0;
                        break;
                    case 11:
                        total += albSupportPaymentObj?.Period12 ?? 0;
                        break;
                }
            }

            return total;
        }

        private decimal? GetYearToDateTotal(LearnerPeriodisedValue albAreaUpliftBalPaymentObj, LearnerPeriodisedValue albAreaUpliftOnProgPaymentObj, int period)
        {
            decimal total = 0;
            for (int i = 0; i < period; i++)
            {
                switch (i)
                {
                    case 0:
                        total += (albAreaUpliftBalPaymentObj?.Period1 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period1 ?? 0);
                        break;
                    case 1:
                        total += (albAreaUpliftBalPaymentObj?.Period2 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period2 ?? 0);
                        break;
                    case 2:
                        total += (albAreaUpliftBalPaymentObj?.Period3 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period3 ?? 0);
                        break;
                    case 3:
                        total += (albAreaUpliftBalPaymentObj?.Period4 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period4 ?? 0);
                        break;
                    case 4:
                        total += (albAreaUpliftBalPaymentObj?.Period5 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period5 ?? 0);
                        break;
                    case 5:
                        total += (albAreaUpliftBalPaymentObj?.Period6 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period6 ?? 0);
                        break;
                    case 6:
                        total += (albAreaUpliftBalPaymentObj?.Period7 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period7 ?? 0);
                        break;
                    case 7:
                        total += (albAreaUpliftBalPaymentObj?.Period8 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period8 ?? 0);
                        break;
                    case 8:
                        total += (albAreaUpliftBalPaymentObj?.Period9 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period9 ?? 0);
                        break;
                    case 9:
                        total += (albAreaUpliftBalPaymentObj?.Period10 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period10 ?? 0);
                        break;
                    case 10:
                        total += (albAreaUpliftBalPaymentObj?.Period11 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period11 ?? 0);
                        break;
                    case 11:
                        total += (albAreaUpliftBalPaymentObj?.Period12 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period12 ?? 0);
                        break;
                }
            }

            return total;
        }
    }
}
