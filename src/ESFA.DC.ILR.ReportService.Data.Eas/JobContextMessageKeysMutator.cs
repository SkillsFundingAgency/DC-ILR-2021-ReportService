using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas
{
    public class JobContextMessageKeysMutator : IJobContextMessageKeysMutator
    {
        private readonly Func<IILR2021_DataStoreEntities> _ilrContext;

        public JobContextMessageKeysMutator(Func<IILR2021_DataStoreEntities> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public async Task<IDictionary<string, object>> MutateAsync(IDictionary<string, object> keyValuePairs, DateTime submissionDateTime, CancellationToken cancellationToken)
        {
            await AddIlrReportingFilename(keyValuePairs, cancellationToken);
            await AddEasReportingFilename(keyValuePairs, submissionDateTime, cancellationToken);

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
                var ilrLastUpdated = fileDetails?.SubmittedTime != null
                    ? fileDetails?.SubmittedTime.Value.ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat)
                    : null;

                keyValuePairs.Add(ReportServiceConstants.IlrReportingFilename, ilrReportingFilename);
                keyValuePairs.Add(ReportServiceConstants.LastIlrFileUpdate, ilrLastUpdated);
            }
        }

        private async Task AddEasReportingFilename(IDictionary<string, object> keyValuePairs, DateTime submissionDateTime, CancellationToken cancellationToken)
        {
            var easReportingFilename = keyValuePairs.ContainsKey(ILRContextKeys.OriginalFilename)
                ? keyValuePairs[ILRContextKeys.OriginalFilename].ToString()
                : keyValuePairs[ILRContextKeys.Filename].ToString();

            keyValuePairs.Add(ILRContextKeys.EasReportingFilename, easReportingFilename);
            keyValuePairs.Add(ILRContextKeys.LastEasFileUpdate, submissionDateTime.ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat));
        }
    }
}
