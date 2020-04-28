using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Data
{
    public class JobContextMessageKeysMutator : IJobContextMessageKeysMutator
    {
        public async Task<IDictionary<string, object>> MutateAsync(IDictionary<string, object> keyValuePairs, CancellationToken cancellationToken)
        {
            await AddIlrReportingFilename(keyValuePairs, cancellationToken);
            await AddEasReportingFilename(keyValuePairs, cancellationToken);

            return keyValuePairs;
        }

        private async Task AddIlrReportingFilename(IDictionary<string, object> keyValuePairs, CancellationToken cancellationToken)
        {
            var ilrReportingFilename = keyValuePairs.ContainsKey(ILRContextKeys.OriginalFilename)
                ? keyValuePairs[ILRContextKeys.OriginalFilename].ToString()
                : keyValuePairs[ILRContextKeys.Filename].ToString();

            keyValuePairs.Add(ReportServiceConstants.IlrReportingFilename, ilrReportingFilename);
        }

        private async Task AddEasReportingFilename(IDictionary<string, object> keyValuePairs, CancellationToken cancellationToken)
        {
            var easReportingFilename = keyValuePairs[ReportServiceConstants.LatestEasFilename]?.ToString();

            keyValuePairs.Add(ReportServiceConstants.EasReportingFilename, easReportingFilename);
        }
    }
}
