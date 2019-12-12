using System;
using System.Globalization;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Abstract
{
    public abstract class AbstractReportModelBuilder
    {
        public string ExtractDisplayDateTimeFromFileName(string ilrFileName)
        {
            var ilrFilenameDateTime = ExtractFileName(ilrFileName).Substring(18, 15);

            return DateTime.TryParseExact(ilrFilenameDateTime, FormattingConstants.IlrFileNameDateTimeParseFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parseDateTime) 
                ? parseDateTime.LongDateStringFormat() 
                : string.Empty;
        }

        public string FormatFilePreparationDate(DateTime filePreparationDate)
        {
            return filePreparationDate.ShortDateStringFormat();
        }

        public string FormatFilePreparationDate(DateTime? filePreparationDate)
        {
            return filePreparationDate?.ShortDateStringFormat();
        }

        public string FormatReportGeneratedAtDateTime(DateTime reportGeneratedAt)
        {
            return reportGeneratedAt.TimeOfDayOnDateStringFormat();
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

        public T RetrieveReportFilterValueFromContext<T>(IReportServiceContext context, string reportName, string propertyName)
        {
            return context
                .ReportFilters?
                .FirstOrDefault(r => r.ReportName.CaseInsensitiveEquals(reportName))?
                .Properties?
                .FirstOrDefault(p => p.PropertyName.CaseInsensitiveEquals(propertyName))?
                .Value is T value 
                ? value 
                : default(T);
        }
    }
}
