using System.Collections.Generic;
using System.Text;
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
            [OutputTypes.Zip] = "zip",
        };

        public FileNameService(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public string GetFilename(IReportServiceContext reportServiceContext, string fileName, OutputTypes outputType, bool includeDateTime = true)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{reportServiceContext.Ukprn}_{reportServiceContext.JobId}_{fileName}");

            if (includeDateTime)
            {
                stringBuilder.Append($" {_dateTimeProvider.ConvertUtcToUk(reportServiceContext.SubmissionDateTimeUtc):yyyyMMdd-HHmmss}");
            }

            stringBuilder.Append($".{GetExtension(outputType)}");

            return stringBuilder.ToString();
        }

        public string GetExtension(OutputTypes outputType)
        {
            return _extensionsDictionary[outputType];
        }
    }
}
