using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop
{
    public class DesktopJobContextMessageKeysMutator : IJobContextMessageKeysMutator
    {
        public async Task<IDictionary<string, object>> MutateAsync(IDictionary<string, object> keyValuePairs, DateTime submissionDateTime, CancellationToken cancellationToken)
        {
            await AddIlrReportingFilename(keyValuePairs, submissionDateTime,  cancellationToken);

            return keyValuePairs;
        }

        private async Task AddIlrReportingFilename(IDictionary<string, object> keyValuePairs, DateTime submissionDateTime, CancellationToken cancellationToken)
        {
            var ilrReportingFilename = keyValuePairs.ContainsKey(ILRContextKeys.OriginalFilename)
                ? keyValuePairs[ILRContextKeys.OriginalFilename].ToString()
                : keyValuePairs[ILRContextKeys.Filename].ToString();

            keyValuePairs.Add(ReportServiceConstants.IlrReportingFilename, ilrReportingFilename);
            keyValuePairs.Add(ReportServiceConstants.LastIlrFileUpdate, submissionDateTime.ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat));
        }
    }
}
