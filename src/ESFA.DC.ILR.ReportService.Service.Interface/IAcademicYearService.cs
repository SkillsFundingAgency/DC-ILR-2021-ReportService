using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IAcademicYearService
    {
        DateTime YearStart { get; }

        DateTime YearEnd { get; }
    }
}
