using System;
using System.Collections;
using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Service
{
    public class FileNameService : IFileNameService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IDictionary<OutputTypes, string> _extensionsDictionary = new Dictionary<OutputTypes, string>()
        {
            [OutputTypes.Csv] = "csv",
            [OutputTypes.Excel] = "xlsx",
            [OutputTypes.Json] = "json",
        };

        public FileNameService(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public string GetFilename(IReportServiceContext reportServiceContext, string fileName, OutputTypes outputType)
        {
            var dateTime = _dateTimeProvider.ConvertUtcToUk(reportServiceContext.SubmissionDateTimeUtc);

            var extension = GetExtension(outputType);

            return $"{reportServiceContext.Ukprn}_{reportServiceContext.JobId}_{fileName} {dateTime:yyyyMMdd-HHmmss}.{extension}";
        }

        public string GetExtension(OutputTypes outputType)
        {
            return _extensionsDictionary[outputType];
        }
    }
}
