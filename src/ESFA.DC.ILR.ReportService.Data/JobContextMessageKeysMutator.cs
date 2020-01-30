using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Data
{
    public class JobContextMessageKeysMutator : IJobContextMessageKeysMutator
    {
        public Task<IDictionary<string, object>> MutateAsync(IDictionary<string, object> keyValuePairs, CancellationToken cancellationToken)
        {
            var ilrReportingFilename = keyValuePairs.ContainsKey(ILRContextKeys.OriginalFilename)
                ? keyValuePairs[ILRContextKeys.OriginalFilename].ToString()
                : keyValuePairs[ILRContextKeys.Filename].ToString();

            keyValuePairs.Add("IlrReportingFilename", ilrReportingFilename);

            return Task.FromResult(keyValuePairs);
        }
    }
}
