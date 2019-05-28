using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Model.PeriodEnd.ILRDataQuality
{
    public class RuleViolationsInfo
    {
        public string RuleName { get; set; }
        public string ErrorMessage { get; set; }
        public int Providers { get; set; }
        public int Learners { get; set; }
        public int NoOfErrors { get; set; }
    }
}
