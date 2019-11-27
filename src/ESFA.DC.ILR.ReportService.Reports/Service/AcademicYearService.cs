
using System;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Service
{
    public class AcademicYearService : IAcademicYearService
    {
        public DateTime YearStart => new DateTime(2019, 8, 1);

        public DateTime YearEnd => new DateTime(2020, 7, 31, 23, 59, 59);
    }
}
