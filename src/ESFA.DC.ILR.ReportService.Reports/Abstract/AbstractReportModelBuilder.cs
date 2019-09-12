using System;
using System.Globalization;

namespace ESFA.DC.ILR.ReportService.Reports.Abstract
{
    public abstract class AbstractReportModelBuilder
    {
        private const string lastSubmittedIlrFileDateStringFormat = "dd/MM/yyyy HH:mm:ss";
        private const string ilrFileNameDateTimeParseFormat = "yyyyMMdd-HHmmss";

        public string ExtractDisplayDateTimeFromFileName(string ilrFileName)
        {
            if (ilrFileName.Length < 33)
            {
                return string.Empty;
            }

            var parts = ilrFileName.Split('/');
            var ilrFilenameDateTime = parts[parts.Length - 1].Substring(18, 15);

            return DateTime.TryParseExact(ilrFilenameDateTime, ilrFileNameDateTimeParseFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parseDateTime) 
                ? parseDateTime.ToString(lastSubmittedIlrFileDateStringFormat) : string.Empty;
        }

        public string ExtractFileName(string ilrFileName)
        {
            if (string.IsNullOrEmpty(ilrFileName) || ilrFileName.Length < 33)
            {
                return string.Empty;
            }

            var parts = ilrFileName.Split('/');
            var ilrFilename = parts[parts.Length - 1];

            return ilrFilename;
        }
    }
}
