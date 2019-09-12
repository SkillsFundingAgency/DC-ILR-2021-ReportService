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
            var parts = ilrFileName.Split('/');
            var ilrFilenameDateTime = parts[parts.Length - 1].Substring(18, 15);

            DateTime.TryParseExact(ilrFilenameDateTime, ilrFileNameDateTimeParseFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parseDateTime);

            return parseDateTime.ToString(lastSubmittedIlrFileDateStringFormat);
        }
    }
}
