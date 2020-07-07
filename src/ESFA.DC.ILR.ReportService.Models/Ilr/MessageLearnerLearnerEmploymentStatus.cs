using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Models.Ilr
{
    public class MessageLearnerLearnerEmploymentStatus : ILearnerEmploymentStatus
    {
        public int EmpStat { get; set; }

        public DateTime DateEmpStatApp { get; set; }

        public int? EmpIdNullable { get; set; }

        public IReadOnlyCollection<IEmploymentStatusMonitoring> EmploymentStatusMonitorings { get; set; }
    }
}
