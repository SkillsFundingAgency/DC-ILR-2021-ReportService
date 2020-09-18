using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System.Linq;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.Summary
{
    public class FrmSummaryReportModelBuilder : FrmBaseModelBuilder, IModelBuilder<FrmSummaryReportModel>
    {
        public FrmSummaryReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();
            var organisationNameDictionary = referenceData.Organisations.ToDictionary(x => x.UKPRN, x => x.Name);
            var orgName = organisationNameDictionary.GetValueOrDefault(reportServiceContext.Ukprn);
            return new FrmSummaryReportModel()
            {
                UKPRN = reportServiceContext.Ukprn,
                ILRFileName = reportServiceContext.OriginalFilename,
                LastFileUpdate = reportServiceContext.LastIlrFileUpdate,
                ProviderName = orgName,
                SecurityClassification = message.HeaderEntity.SourceEntity.ProtectiveMarkingString
            };
        }


    }
}
