using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IStringUtilitiesService
    {
        string JoinWithMaxLength(IEnumerable<string> list);

        DateTime? GetIlrFileDate(string ilrFilename);

        int TryGetInt(string value, int def);

        string GetDateTimeAsString(DateTime? dateTime, string def, DateTime? replace = null);
    }
}
