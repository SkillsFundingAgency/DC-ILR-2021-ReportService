using System;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IAcademicYearService
    {
        DateTime YearStart { get; }

        DateTime YearEnd { get; }
    }
}
