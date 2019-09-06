using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim
{
    public class FundingClaimReportModelBuilder : IModelBuilder<FundingClaimReportModel>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private const string ReportGeneratedTimeStringFormat = "HH:mm:ss on dd/MM/yyyy";

        public FundingClaimReportModelBuilder(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }
        public FundingClaimReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm25Data = reportServiceDependentData.Get<FM25Global>();
            var referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();

            var organisationName = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.Name ?? string.Empty;

            var cofRemoval = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.OrganisationCoFRemovals?.OrderByDescending(x => x.EffectiveFrom).FirstOrDefault()?.CoFRemoval;

            var learners = message?.Learners ?? Enumerable.Empty<ILearner>();
            var model = new FundingClaimReportModel();

            var ilrFileName = reportServiceContext.OriginalFilename ?? reportServiceContext.Filename;

            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            var reportGeneratedAt = "Report generated at: " + dateTimeNowUk.ToString(ReportGeneratedTimeStringFormat);

            // Header
            model.ProviderName = organisationName;
            model.Ukprn = reportServiceContext.Ukprn;
            model.IlrFile = ilrFileName;
            model.Year = ReportingConstants.Year;

           
            // Body
        

            // Footer
            model.ReportGeneratedAt = reportGeneratedAt;
            model.ApplicationVersion = reportServiceContext.ServiceReleaseVersion;
            model.ComponentSetVersion = "NA";
            model.OrganisationData = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            model.LargeEmployerData = referenceDataRoot.MetaDatas.ReferenceDataVersions.Employers.Version;
            model.LarsData = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            model.PostcodeData = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;
            model.FilePreparationDate = message.HeaderEntity.CollectionDetailsEntity.FilePreparationDate.ToString("dd/MM/yyyy");

            return model;
        }
    }
}
