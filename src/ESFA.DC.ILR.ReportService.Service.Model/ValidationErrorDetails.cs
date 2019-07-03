using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Service.Model
{
    public sealed class ValidationErrorDetails
    {
        public string RuleName { get; }

        public string Message { get; set; }

        public string Severity { get; set; }

        public ValidationErrorDetails(string ruleName)
        {
            RuleName = ruleName;
        }
    }
}
