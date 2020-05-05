using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS2021.EF.Interface;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data
{
    public class JobContextMessageKeysMutator : IJobContextMessageKeysMutator
    {
        private readonly Func<IEasdbContext> _easContext;

        public JobContextMessageKeysMutator(Func<IEasdbContext> easContext)
        {
            _easContext = easContext;
        }

        public async Task<IDictionary<string, object>> MutateAsync(IDictionary<string, object> keyValuePairs, DateTime submissionDateTime, CancellationToken cancellationToken)
        {
            await AddIlrReportingFilename(keyValuePairs, submissionDateTime,  cancellationToken);
            await AddEasReportingFilename(keyValuePairs, cancellationToken);

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

        private async Task AddEasReportingFilename(IDictionary<string, object> keyValuePairs, CancellationToken cancellationToken)
        {
            var ukprn = keyValuePairs[ILRContextKeys.Ukprn].ToString();

            using (var easContext = _easContext())
            {
                var fileDetails = await easContext.SourceFiles
                    .Where(fd => fd.Ukprn == ukprn)
                    .OrderByDescending(d => d.FilePreparationDate)
                    .FirstOrDefaultAsync(cancellationToken);

                var easReportingFilename = Path.GetFileName(fileDetails?.FileName);
                var easLastUpdated = fileDetails?.FilePreparationDate != null
                    ? fileDetails?.FilePreparationDate.ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat)
                    : null;

                keyValuePairs.Add(ILRContextKeys.EasReportingFilename, easReportingFilename);
                keyValuePairs.Add(ILRContextKeys.LastEasFileUpdate, easLastUpdated);
            }
        }
    }
}
