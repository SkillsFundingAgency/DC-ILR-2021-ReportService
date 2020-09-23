using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Frm.Model;
using ESFA.DC.ILR.ReportService.Reports.Frm.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public class FrmReportModelBuilder : FrmBaseModelBuilder, IModelBuilder<IFrmSummaryReport>
    {
        public IFrmSummaryReport Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();
            var organisationNameDictionary = referenceData.Organisations.ToDictionary(x => x.UKPRN, x => x.Name);
            var orgName = organisationNameDictionary.GetValueOrDefault(reportServiceContext.Ukprn);
            return new FrmSummaryReportModel(BuildHeader(
                orgName,
                reportServiceContext.Ukprn.ToString(),
                reportServiceContext.OriginalFilename.Split('/').Last(),
                reportServiceContext.LastIlrFileUpdate,
                message.HeaderEntity.SourceEntity.ProtectiveMarkingString));
        }

        public IDictionary<string, string> BuildHeader(string providerName, string UKPRN, string ilrFileName, string lastIlrFileUpdate, string securityClassification)
        {
            return new Dictionary<string, string>
            {
                { "Provider Name:", providerName },
                { "UKPRN:", UKPRN },
                { "ILR File:",  ilrFileName },
                { "Last ILR File Update:", lastIlrFileUpdate },
                { "Security Classification", securityClassification }
            };
        }
    }
}
