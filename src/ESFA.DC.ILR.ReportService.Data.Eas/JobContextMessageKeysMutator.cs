using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas
{
    public class JobContextMessageKeysMutator : IJobContextMessageKeysMutator
    {
        private readonly Func<IILR1920_DataStoreEntities> _ilrContext;

        public JobContextMessageKeysMutator(Func<IILR1920_DataStoreEntities> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public async Task<IDictionary<string, object>> MutateAsync(IDictionary<string, object> keyValuePairs, CancellationToken cancellationToken)
        {
            await AddIlrReportingFilename(keyValuePairs, cancellationToken);
            
            return keyValuePairs;
        }

        private async Task AddIlrReportingFilename(IDictionary<string, object> keyValuePairs, CancellationToken cancellationToken)
        {
            var ukprn = int.Parse(keyValuePairs[ILRContextKeys.Ukprn].ToString());

            using (var ilrContext = _ilrContext())
            {
                var fileDetails = await ilrContext.FileDetails
                    .Where(fd => fd.UKPRN == ukprn)
                    .OrderByDescending(d => d.SubmittedTime)
                    .FirstOrDefaultAsync(cancellationToken);

                var ilrReportingFilename = Path.GetFileName(fileDetails?.Filename);

                keyValuePairs.Add("IlrReportingFilename", ilrReportingFilename);
            }
        }
    }
}
