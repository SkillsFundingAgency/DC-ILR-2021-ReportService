using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Trailblazer
{
    public class TrailblazerOccupancyReportModelBuilder : AbstractOccupancyReportModelBuilder, IModelBuilder<IEnumerable<TrailblazerOccupancyReportModel>>
    {
        public TrailblazerOccupancyReportModelBuilder(IIlrModelMapper ilrModelMapper)
            : base(ilrModelMapper)
        {           
        }

        public IEnumerable<TrailblazerOccupancyReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm81 = reportServiceDependentData.Get<FM81Global>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var larsLearningDeliveries = BuildLarsLearningDeliveryDictionary(referenceData);
            var fm81LearningDeliveryDictionary = BuildFm81LearningDeliveryDictionary(fm81);

            var models = new List<TrailblazerOccupancyReportModel>();

            foreach (var learner in message?.Learners?.Where(l => l != null) ?? Enumerable.Empty<ILearner>())
            {
                foreach (var learningDelivery in learner.LearningDeliveries ?? Enumerable.Empty<ILearningDelivery>())
                {
                    if (Filter(learningDelivery))
                    {
                        var larsLearningDelivery = larsLearningDeliveries.GetValueOrDefault(learningDelivery.LearnAimRef);
                        var providerSpecLearnerMonitoring = _ilrModelMapper.MapProviderSpecLearnerMonitorings(learner.ProviderSpecLearnerMonitorings);
                        var providerSpecDeliveryMonitoring = _ilrModelMapper.MapProviderSpecDeliveryMonitorings(learningDelivery.ProviderSpecDeliveryMonitorings);
                        var learningDeliveryFams = _ilrModelMapper.MapLearningDeliveryFAMs(learningDelivery.LearningDeliveryFAMs);
                        var fm81LearningDelivery = fm81LearningDeliveryDictionary.GetValueOrDefault(learner.LearnRefNumber).GetValueOrDefault(learningDelivery.AimSeqNumber);
                        var periodisedValues = BuildFm81PeriodisedValuesModel(fm81LearningDelivery?.LearningDeliveryPeriodisedValues);

                        models.Add(new TrailblazerOccupancyReportModel()
                        {
                            Learner = learner,
                            LearningDelivery = learningDelivery,
                            LarsLearningDelivery = larsLearningDelivery,
                            ProviderSpecLearnerMonitoring = providerSpecLearnerMonitoring,                           
                            ProviderSpecDeliveryMonitoring = providerSpecDeliveryMonitoring,
                            LearningDeliveryFAMs = learningDeliveryFams,
                            Fm81LearningDelivery = fm81LearningDelivery?.LearningDeliveryValues,
                            AppFinRecord = BuildAppFinRecordModel(learningDelivery),
                            PeriodisedValues = periodisedValues,
                        });
                    }
                }
            }

            return Order(models);
        }

        public bool Filter(ILearningDelivery learningDelivery)
        {
            if (learningDelivery != null)
            {
                return learningDelivery.FundModel == FundModelConstants.FM81
                       && learningDelivery.ProgTypeNullable == LearningProgrammeConstants.ApprenticeshipStandard;
            }

            return false;
        }

        private Dictionary<string, Dictionary<int?, LearningDelivery>> BuildFm81LearningDeliveryDictionary(FM81Global fm81Global)
        {
            return fm81Global?
                       .Learners?
                       .ToDictionary(
                           l => l.LearnRefNumber,
                           l => l.LearningDeliveries
                               .ToDictionary(
                                   ld => ld.AimSeqNumber,
                                   ld => ld),
                           StringComparer.OrdinalIgnoreCase)
                   ?? new Dictionary<string, Dictionary<int?, LearningDelivery>>();
        }

        private AppFinRecordModel BuildAppFinRecordModel(ILearningDelivery learningDelivery)
        {
            var appFinRecords = learningDelivery?.AppFinRecords;

            IEnumerable<IAppFinRecord> previousYearPmrData = appFinRecords?
                .Where(x => x.AFinDate <= ReportingConstants.BeginningOfYear &&
                            x.AFinType.CaseInsensitiveEquals(AppFinRecordConstants.Types.PaymentRecord));

            IEnumerable<IAppFinRecord> currentYearPmrData = appFinRecords?
                .Where(x => x.AFinDate >= ReportingConstants.BeginningOfYear &&
                            x.AFinDate <= ReportingConstants.EndOfYear &&
                            x.AFinType.CaseInsensitiveEquals(AppFinRecordConstants.Types.PaymentRecord));

            return new AppFinRecordModel
            {
                LatestTotalNegotiatedPrice1 = GetLatestTnpPriceFor(learningDelivery, appFinRecords, AppFinRecordConstants.TotalNegotiatedPriceCodes.TotalTrainingPrice),
                LatestTotalNegotiatedPrice2 = GetLatestTnpPriceFor(learningDelivery, appFinRecords, AppFinRecordConstants.TotalNegotiatedPriceCodes.TotalAssessmentPrice),
                SumOfPmrsBeforeFundingYear = previousYearPmrData?.Where(x => x.AFinCode == 1 || x.AFinCode == 2).Sum(x => x.AFinAmount) - previousYearPmrData?.Where(x => x.AFinCode == 3).Sum(x => x.AFinAmount),
                SumOfAugustPmrs = GetSumOfPmrsFor(currentYearPmrData, 08, ReportingConstants.BeginningOfYear.Year),
                SumOfSeptemberPmrs = GetSumOfPmrsFor(currentYearPmrData, 09, ReportingConstants.BeginningOfYear.Year),
                SumOfOctoberPmrs = GetSumOfPmrsFor(currentYearPmrData, 10, ReportingConstants.BeginningOfYear.Year),
                SumOfNovemberPmrs = GetSumOfPmrsFor(currentYearPmrData, 11, ReportingConstants.BeginningOfYear.Year),
                SumOfDecemberPmrs = GetSumOfPmrsFor(currentYearPmrData, 12, ReportingConstants.BeginningOfYear.Year),
                SumOfJanuaryPmrs = GetSumOfPmrsFor(currentYearPmrData, 01, ReportingConstants.EndOfYear.Year),
                SumOfFebruaryPmrs = GetSumOfPmrsFor(currentYearPmrData, 02, ReportingConstants.EndOfYear.Year),
                SumOfMarchPmrs = GetSumOfPmrsFor(currentYearPmrData, 03, ReportingConstants.EndOfYear.Year),
                SumOfAprilPmrs = GetSumOfPmrsFor(currentYearPmrData, 04, ReportingConstants.EndOfYear.Year),
                SumOfMayPmrs = GetSumOfPmrsFor(currentYearPmrData, 05, ReportingConstants.EndOfYear.Year),
                SumOfJunePmrs = GetSumOfPmrsFor(currentYearPmrData, 06, ReportingConstants.EndOfYear.Year),
                SumOfJulyPmrs = GetSumOfPmrsFor(currentYearPmrData, 07, ReportingConstants.EndOfYear.Year),
                PmrsTotal = currentYearPmrData?.Where(x => x.AFinCode == 1 || x.AFinCode == 2).Sum(x => x.AFinAmount) - currentYearPmrData?.Where(x => x.AFinCode == 3).Sum(x => x.AFinAmount)
            };
        }

        private decimal? GetLatestTnpPriceFor(ILearningDelivery learningDelivery, IEnumerable<IAppFinRecord> appFinRecords, int appFinCode)
        {
            if (learningDelivery.AimType != 1)
            {
                return null;
            }

            return appFinRecords?.Where(x =>
                    x.AFinCode == appFinCode &&
                    x.AFinType.CaseInsensitiveEquals(AppFinRecordConstants.Types.TotalNegotiatedPrice))
                .OrderByDescending(x => x.AFinDate)
                .FirstOrDefault()?.AFinAmount;
        }

        private decimal? GetSumOfPmrsFor(IEnumerable<IAppFinRecord> appFinRecords, int month, int year)
        {
            var records = appFinRecords?.Where(x => x.AFinDate.Month == month && x.AFinDate.Year == year &&
                                               x.AFinType.CaseInsensitiveEquals(AppFinRecordConstants.Types.PaymentRecord));

            return records?.Where(x => x.AFinCode == 1 || x.AFinCode == 2).Sum(x => x.AFinAmount) -
                             records?.Where(x => x.AFinCode == 3).Sum(x => x.AFinAmount);

        }
    }
}
