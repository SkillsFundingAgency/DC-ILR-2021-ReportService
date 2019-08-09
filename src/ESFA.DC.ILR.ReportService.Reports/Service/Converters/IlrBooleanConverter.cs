using System;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using ESFA.DC.ILR.ReportService.Reports.Constants;

namespace ESFA.DC.ILR.ReportService.Reports.Service.Converters
{
    public class IlrBooleanConverter : ITypeConverter
    {
        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is bool booleanValue)
            {
                return booleanValue ? ReportingConstants.Y : ReportingConstants.N;
            }

            return string.Empty;
        }

        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException();
        }
    }
}
