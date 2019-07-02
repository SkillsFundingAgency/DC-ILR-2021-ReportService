using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Validation
{
    public sealed class ValidationErrorsModelComparer : IComparer<ValidationErrorRow>
    {
        public int Compare(ValidationErrorRow x, ValidationErrorRow y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (x == y)
            {
                return 0;
            }

            if (IsError(x) && !IsError(y))
            {
                return -1;
            }

            if (!IsError(x) && IsError(y))
            {
                return 1;
            }

            return string.Compare(x.RuleName, y.RuleName, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsError(ValidationErrorRow x)
        {
            return string.Equals(x.Severity, "E", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(x.Severity, "F", StringComparison.OrdinalIgnoreCase);
        }
    }
}
