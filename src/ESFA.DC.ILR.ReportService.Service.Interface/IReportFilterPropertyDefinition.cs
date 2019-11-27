using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReportFilterPropertyDefinition
    {
        string Name { get; }

        string Type { get; }
    }
}
