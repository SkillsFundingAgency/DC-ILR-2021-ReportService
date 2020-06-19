using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Models.Ilr
{
    public class MessageLearnerLearnerEmploymentStatus
    {
        public DateTime DateEmpStatApp { get; set; }

        public int EmpStat { get; set; }

        public MessageLearnerLearnerEmploymentStatusEmploymentStatusMonitoring EsmMonitoring { get; set; }
    }
}
