using System;

namespace ESFA.DC.ILR.ReportService.Reports.Constants
{
    public static class ReportingConstants
    {
        public const string OfficialSensitive = "OFFICIAL-SENSITIVE";
        public const string Yes = "Yes";
        public const string No = "No";

        public const string EmploymentStatusMonitoringTypeSEM = "SEM";

        public const string Y = "Y";
        public const string N = "N";

        // Dates
        public static readonly DateTime BeginningOfYear = new DateTime(2019, 8, 1);
        public static readonly DateTime EndOfYear = new DateTime(2020, 7, 31, 23, 59, 59);
    }
}
