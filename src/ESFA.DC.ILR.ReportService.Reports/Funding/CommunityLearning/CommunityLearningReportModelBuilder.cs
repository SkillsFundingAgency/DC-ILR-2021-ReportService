using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning
{
    public class CommunityLearningReportModelBuilder : AbstractReportModelBuilder, IModelBuilder<CommunityLearningReportModel>
    {
        public CommunityLearningReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            return BuildModel();
        }

        public CommunityLearningReportModel BuildModel()
        {

            return new CommunityLearningReportModel();
        }

        private IDictionary<string, string> BuildHeaderData(IReportServiceContext reportServiceContext, ReferenceDataRoot referenceDataRoot)
        {
            var organisationName = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.Name ?? string.Empty;
            var easLastUpdate = referenceDataRoot.MetaDatas.ReferenceDataVersions?.EasUploadDateTime.UploadDateTime.ToString();

            return new Dictionary<string, string>()
            {
                {SummaryPageConstants.ProviderName, organisationName},
                {SummaryPageConstants.UKPRN, reportServiceContext.Ukprn.ToString()},
                {SummaryPageConstants.ILRFile, reportServiceContext.OriginalFilename},
                {SummaryPageConstants.LastILRFileUpdate, ExtractDisplayDateTimeFromFileName(reportServiceContext.OriginalFilename)},
                {SummaryPageConstants.LastEASUpdate, easLastUpdate},
                {SummaryPageConstants.SecurityClassification, ReportingConstants.OfficialSensitive}
            };
        }
    }
}
