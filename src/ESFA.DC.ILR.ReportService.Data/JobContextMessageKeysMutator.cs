using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Data
{
    public class JobContextMessageKeysMutator : IJobContextMessageKeysMutator
    {
        public async Task<IDictionary<string, object>> Mutate(IDictionary<string, object> keyValuePairs, CancellationToken cancellationToken)
        {
            await AddIlrReportingFilename(keyValuePairs);

            return keyValuePairs;
        }

        private async Task AddIlrReportingFilename(IDictionary<string, object> keyValuePairs)
        {
            var ilrReportingFilename = keyValuePairs.ContainsKey("OriginalFilename")
                ? keyValuePairs["OriginalFilename"].ToString()
                : keyValuePairs[ILRContextKeys.Filename].ToString();

            keyValuePairs.Add("IlrReportingFilename", ilrReportingFilename);
        }
    }
}
