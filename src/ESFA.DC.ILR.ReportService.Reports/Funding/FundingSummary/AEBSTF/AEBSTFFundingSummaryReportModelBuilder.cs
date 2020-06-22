using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.AEBSTF
{
    public class AEBSTFFundingSummaryReportModelBuilder : AbstractReportModelBuilder, IModelBuilder<AEBSTFFundingSummaryReportModel>
    {
        private readonly IPeriodisedValuesLookupProvider _periodisedValuesLookupProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        protected IEnumerable<FundingDataSources> FundingDataSources { private get; set; }

        public AEBSTFFundingSummaryReportModelBuilder(IPeriodisedValuesLookupProvider periodisedValuesLookupProvider, IDateTimeProvider dateTimeProvider)
        {
            _periodisedValuesLookupProvider = periodisedValuesLookupProvider;
            _dateTimeProvider = dateTimeProvider;

            FundingDataSources = new[]
            {
                Funding.FundingDataSources.FM35,
                Funding.FundingDataSources.EAS,
            };
        }

        public AEBSTFFundingSummaryReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();

            var periodisedValues = _periodisedValuesLookupProvider.Provide(FundingDataSources, reportServiceDependentData);

            var reportCurrentPeriod = reportServiceContext.ReturnPeriod > 12 ? 12 : reportServiceContext.ReturnPeriod;

            var description = "ESFA AEB - Short Term Funding Initiative";

            return new AEBSTFFundingSummaryReportModel(
                BuildHeaderData(reportServiceContext, referenceDataRoot),
                new List<IFundingCategory>()
                {
                    new FundingCategory(
                        @"ESFA Adult Education Budget - Short Term Funding Initiatives", reportCurrentPeriod,
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory($@"{description} 1", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrStfiFundLineGroup($@"{description} 1", reportCurrentPeriod, new [] { FundLineConstants.STFI1 }, periodisedValues))
                                .WithFundLineGroup(BuildEasStfiFundLineGroup($@"{description} 1", reportCurrentPeriod, new [] { FundLineConstants.STFI1 }, periodisedValues)),
                            new FundingSubCategory($@"{description} 2", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrStfiFundLineGroup($@"{description} 2", reportCurrentPeriod, new [] { FundLineConstants.STFI2 }, periodisedValues))
                                .WithFundLineGroup(BuildEasStfiFundLineGroup($@"{description} 2", reportCurrentPeriod, new [] { FundLineConstants.STFI2 }, periodisedValues)),
                            new FundingSubCategory($@"{description} 3", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrStfiFundLineGroup($@"{description} 3", reportCurrentPeriod, new [] { FundLineConstants.STFI3 }, periodisedValues))
                                .WithFundLineGroup(BuildEasStfiFundLineGroup($@"{description} 3", reportCurrentPeriod, new [] { FundLineConstants.STFI3 }, periodisedValues)),
                            new FundingSubCategory($@"{description} 4", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrStfiFundLineGroup($@"{description} 4", reportCurrentPeriod, new [] { FundLineConstants.STFI4 }, periodisedValues))
                                .WithFundLineGroup(BuildEasStfiFundLineGroup($@"{description} 4", reportCurrentPeriod, new [] { FundLineConstants.STFI4 }, periodisedValues))
                        })
                },
                BuildFooterData(reportServiceContext, message, referenceDataRoot));
        }

        private IDictionary<string, string> BuildHeaderData(IReportServiceContext reportServiceContext, ReferenceDataRoot referenceDataRoot)
        {
            var organisationName = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.Name ?? string.Empty;

            return new Dictionary<string, string>()
            {
                {SummaryPageConstants.ProviderName, organisationName},
                {SummaryPageConstants.UKPRN, reportServiceContext.Ukprn.ToString()},
                {SummaryPageConstants.ILRFile, reportServiceContext.IlrReportingFilename},
                {SummaryPageConstants.LastILRFileUpdate, reportServiceContext.LastIlrFileUpdate},
                {SummaryPageConstants.EASFile, reportServiceContext.EasReportingFilename},
                {SummaryPageConstants.LastEASFileUpdate, reportServiceContext.LastEasFileUpdate },
                {SummaryPageConstants.SecurityClassification, ReportingConstants.OfficialSensitive}
            };
        }

        private IDictionary<string, string> BuildFooterData(IReportServiceContext reportServiceContext, IMessage message, ReferenceDataRoot referenceDataRoot)
        {
            var filePreparationDate = message?.HeaderEntity?.CollectionDetailsEntity?.FilePreparationDate.ShortDateStringFormat();
            var orgVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            var larsVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            var employersVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.Employers.Version;
            var postcodesVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;
            var applicationversion = reportServiceContext.ServiceReleaseVersion;

            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            var reportGeneratedAt = dateTimeNowUk.TimeOfDayOnDateStringFormat();

            return new Dictionary<string, string>()
            {
                {SummaryPageConstants.ApplicationVersion, reportServiceContext.ServiceReleaseVersion},
                {SummaryPageConstants.FilePreparationDate, filePreparationDate},
                {SummaryPageConstants.LARSVersion, larsVersion},
                {SummaryPageConstants.PostcodeVersion, postcodesVersion},
                {SummaryPageConstants.OrganisationVersion, orgVersion},
                {SummaryPageConstants.LargeEmployersVersion, employersVersion},
                {SummaryPageConstants.ReportGeneratedAt, reportGeneratedAt}
          
            };
        }

        private IFundLineGroup BuildIlrStfiFundLineGroup(string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total {description} (£)", currentPeriod, Funding.FundingDataSources.FM35, fundLines, periodisedValues)
                .WithFundLine($"ILR {description} Programme Funding (£)", new[] { AttributeConstants.Fm35OnProgPayment, AttributeConstants.Fm35AchievePayment, AttributeConstants.Fm35EmpOutcomePay, AttributeConstants.Fm35BalancePayment })
                .WithFundLine($"ILR {description} Learning Support (£)", new[] { AttributeConstants.Fm35LearnSuppFundCash });
        }

        protected virtual IFundLineGroup BuildEasStfiFundLineGroup(string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"EAS Total {description} Earnings Adjustment (£)", currentPeriod, Funding.FundingDataSources.EAS, fundLines, periodisedValues)
                .WithFundLine($"EAS {description} Authorised Claims (£)", new[] { AttributeConstants.EasAuthorisedClaims });
        }
    }
}
