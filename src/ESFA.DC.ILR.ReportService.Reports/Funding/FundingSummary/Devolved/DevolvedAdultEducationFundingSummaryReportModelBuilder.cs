using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved
{
    public class DevolvedAdultEducationFundingSummaryReportModelBuilder : IModelBuilder<IEnumerable<DevolvedAdultEducationFundingSummaryReportModel>>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private const string reportGeneratedTimeStringFormat = "HH:mm:ss on MM/dd/yyyy";

        private readonly IEnumerable<string> _sofLearnDelFamCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.SOF_GreaterManchesterCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_LiverpoolCityRegionCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_WestMidlandsCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_WestOfEnglandCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_TeesValleyCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_CambridgeshireAndPeterboroughCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_GreaterLondonAuthority,
        };

        public DevolvedAdultEducationFundingSummaryReportModelBuilder(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public IEnumerable<DevolvedAdultEducationFundingSummaryReportModel> Build(IReportServiceContext reportServiceContext,
            IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm35 = reportServiceDependentData.Get<FM35Global>();
            var referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();

            var sofCodesLookups = referenceDataRoot.DevolvedPostocdes.McaGlaSofLookups;

            var organisationName = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.Name ?? string.Empty;

            var orgVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            var larsVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            var employersVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.Employers.Version;
            var postcodesVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;
            var easLastUpdate = referenceDataRoot.MetaDatas.ReferenceDataVersions?.EasUploadDateTime.UploadDateTime.ToString();

            var filePreparationDate = message.HeaderEntity.CollectionDetailsEntity.FilePreparationDate;

            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            var reportGeneratedAt = dateTimeNowUk.ToString(reportGeneratedTimeStringFormat);

            var reportCurrentPeriod = reportServiceContext.ReturnPeriod > 12 ? 12 : reportServiceContext.ReturnPeriod;

            var models = new List<DevolvedAdultEducationFundingSummaryReportModel>();

            foreach (var sofCode in _sofLearnDelFamCodes)
            {
                var mgaClaSof = sofCodesLookups.FirstOrDefault(s => s.SofCode == sofCode, StringComparison.OrdinalIgnoreCase);
                var learningDeliveries = BuildLearningDeliveryDictionary(message, mgaClaSof?.SofCode);

                var periodisedValues = new PeriodisedValuesLookup
                {
                    [FundingDataSources.FM35] = BuildPeriodisedValuesDictionary(fm35, learningDeliveries),
                    [FundingDataSources.EAS] = BuildEASDictionary(referenceDataRoot, mgaClaSof?.SofCode)
                };

                models.Add(new DevolvedAdultEducationFundingSummaryReportModel(
                    mgaClaSof,
                    reportServiceContext.Ukprn,
                    organisationName,
                    reportServiceContext.OriginalFilename,
                    reportServiceContext.OriginalFilename,
                    filePreparationDate,
                    easLastUpdate,
                    orgVersion,
                    larsVersion,
                    postcodesVersion,
                    employersVersion,
                    reportServiceContext.ServiceReleaseVersion,
                    reportGeneratedAt,
                    new List<IDevolvedAdultEducationFundingCategory>
                    {
                        new DevolvedAdultEducationFundingCategory
                        (@"Adult Education Budget - Eligible for MCA/GLA funding (non-procured)", reportCurrentPeriod,
                            new List<IDevolvedAdultEducationFundLineGroup>
                            {
                                BuildIlrFm35FundLineGroup(reportCurrentPeriod, new [] { FundLineConstants.AdultEducationEligibleMCAGLANonProcured }, periodisedValues),
                                BuildEasFm35FundLineGroup(reportCurrentPeriod, new [] { FundLineConstants.AdultEducationEligibleMCAGLANonProcured }, periodisedValues)
                            }),
                        new DevolvedAdultEducationFundingCategory
                        (@"Adult Education Budget - Eligible for MCA/GLA funding (procured)", reportCurrentPeriod,
                            new List<IDevolvedAdultEducationFundLineGroup>
                            {
                                BuildIlrFm35FundLineGroup(reportCurrentPeriod, new [] { FundLineConstants.AdultEducationEligibleMCAGLAProcured }, periodisedValues),
                                BuildEasFm35FundLineGroup(reportCurrentPeriod, new [] { FundLineConstants.AdultEducationEligibleMCAGLAProcured }, periodisedValues)
                            })
                    }
                ));
            }

            return models;
        }

        private IDevolvedAdultEducationFundLineGroup BuildIlrFm35FundLineGroup(int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new DevolvedAdultEducationFundLineGroup($"ILR Total Adult Education (£)", currentPeriod, FundingDataSources.FM35, fundLines, periodisedValues)
                .WithFundLine($"ILR Programme Funding (£)", new[] { AttributeConstants.Fm35OnProgPayment, AttributeConstants.Fm35AchievePayment, AttributeConstants.Fm35EmpOutcomePay, AttributeConstants.Fm35BalancePayment })
                .WithFundLine($"ILR Learning Support (£)", new[] { AttributeConstants.Fm35LearnSuppFundCash });
        }

        private IDevolvedAdultEducationFundLineGroup BuildEasFm35FundLineGroup(int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new DevolvedAdultEducationFundLineGroup("EAS Total Earnings Adjustment (£)", currentPeriod, FundingDataSources.EAS, fundLines, periodisedValues)
                .WithFundLine("EAS Authorised Claims (£)", new [] { AttributeConstants.EasAuthorisedClaims })
                .WithFundLine("EAS Prince's Trust (£)", new [] { AttributeConstants.EasPrincesTrust })
                .WithFundLine("EAS Excess Learning Support (£)", new [] { AttributeConstants.EasExcessLearningSupport })
                .WithFundLine("EAS MCA/GLA Defined Adjustment 1 (£)", new [] { AttributeConstants.EasMcaGlaDefinedAdjustment1 })
                .WithFundLine("EAS MCA/GLA Defined Adjustment 2 (£)", new[] { AttributeConstants.EasMcaGlaDefinedAdjustment2 })
                .WithFundLine("EAS MCA/GLA Defined Adjustment 3 (£)", new[] { AttributeConstants.EasMcaGlaDefinedAdjustment3 });
        }

        private IDictionary<string, Dictionary<int, ILearningDelivery>> BuildLearningDeliveryDictionary(IMessage message, string sofFamCode)
        {
            return message?
                       .Learners?
                       .ToDictionary(
                           l => l.LearnRefNumber,
                           l => l.LearningDeliveries
                               .Where(ld => ld.LearningDeliveryFAMs.Any(ldf =>
                                   ldf.LearnDelFAMType == LearningDeliveryFAMTypeConstants.SOF &&
                                   ldf.LearnDelFAMCode == sofFamCode))
                               .ToDictionary(
                                   ld => ld.AimSeqNumber,
                                   ld => ld),
                           StringComparer.OrdinalIgnoreCase)
                   ?? new Dictionary<string, Dictionary<int, ILearningDelivery>>();
        }

        public Dictionary<string, Dictionary<string, decimal?[][]>> BuildPeriodisedValuesDictionary(FM35Global fm35Global, IDictionary<string, Dictionary<int, ILearningDelivery>> learningDeliveries)
        {
            return fm35Global?
                       .Learners?
                       .Where(l => learningDeliveries.ContainsKey(l.LearnRefNumber))
                       .SelectMany(l => l.LearningDeliveries.Where(ld => ld.AimSeqNumber.HasValue
                                                                         && learningDeliveries[l.LearnRefNumber].ContainsKey(ld.AimSeqNumber.Value)))
                       .GroupBy(ld => ld.LearningDeliveryValue.FundLine, StringComparer.OrdinalIgnoreCase)
                       .ToDictionary(k => k.Key,
                           v => v.SelectMany(ld => ld.LearningDeliveryPeriodisedValues)
                               .GroupBy(ldpv => ldpv.AttributeName, StringComparer.OrdinalIgnoreCase)
                               .ToDictionary(k => k.Key, value =>
                                       value.Select(pvGroup => new decimal?[]
                                       {
                                           pvGroup.Period1,
                                           pvGroup.Period2,
                                           pvGroup.Period3,
                                           pvGroup.Period4,
                                           pvGroup.Period5,
                                           pvGroup.Period6,
                                           pvGroup.Period7,
                                           pvGroup.Period8,
                                           pvGroup.Period9,
                                           pvGroup.Period10,
                                           pvGroup.Period11,
                                           pvGroup.Period12,
                                       }).ToArray(),
                                   StringComparer.OrdinalIgnoreCase),
                           StringComparer.OrdinalIgnoreCase)
                   ?? new Dictionary<string, Dictionary<string, decimal?[][]>>();
        }

        public Dictionary<string, Dictionary<string, decimal?[][]>> BuildEASDictionary(ReferenceDataRoot referenceDataRoot, string sofCode)
        {
            int.TryParse(sofCode, out var sofCodeInt);

            return referenceDataRoot?
                       .EasFundingLines?
                       .GroupBy(fl => fl.FundLine, StringComparer.OrdinalIgnoreCase)
                       .ToDictionary(k => k.Key,
                           v => v.SelectMany(ld => ld.EasSubmissionValues)
                               .GroupBy(easv => easv.AdjustmentTypeName, StringComparer.OrdinalIgnoreCase)
                               .ToDictionary(k => k.Key, value =>
                                       value.Select(pvGroup => new decimal?[]
                                       {
                                           pvGroup.Period1.DevolvedAreaSof == sofCodeInt ? pvGroup.Period1.PaymentValue : 0.0m,
                                           pvGroup.Period2.DevolvedAreaSof == sofCodeInt ? pvGroup.Period2.PaymentValue : 0.0m,
                                           pvGroup.Period3.DevolvedAreaSof == sofCodeInt ? pvGroup.Period3.PaymentValue : 0.0m,
                                           pvGroup.Period4.DevolvedAreaSof == sofCodeInt ? pvGroup.Period4.PaymentValue : 0.0m,
                                           pvGroup.Period5.DevolvedAreaSof == sofCodeInt ? pvGroup.Period5.PaymentValue : 0.0m,
                                           pvGroup.Period6.DevolvedAreaSof == sofCodeInt ? pvGroup.Period6.PaymentValue : 0.0m,
                                           pvGroup.Period7.DevolvedAreaSof == sofCodeInt ? pvGroup.Period7.PaymentValue : 0.0m,
                                           pvGroup.Period8.DevolvedAreaSof == sofCodeInt ? pvGroup.Period8.PaymentValue : 0.0m,
                                           pvGroup.Period9.DevolvedAreaSof == sofCodeInt ? pvGroup.Period9.PaymentValue : 0.0m,
                                           pvGroup.Period10.DevolvedAreaSof == sofCodeInt ? pvGroup.Period10.PaymentValue : 0.0m,
                                           pvGroup.Period11.DevolvedAreaSof == sofCodeInt ? pvGroup.Period11.PaymentValue : 0.0m,
                                           pvGroup.Period12.DevolvedAreaSof == sofCodeInt ? pvGroup.Period12.PaymentValue : 0.0m,
                                       }).ToArray(),
                                   StringComparer.OrdinalIgnoreCase),
                           StringComparer.OrdinalIgnoreCase)
                   ?? new Dictionary<string, Dictionary<string, decimal?[][]>>();
        }
    }
}
