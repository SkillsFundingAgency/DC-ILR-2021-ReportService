using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR1819.ReportService.Service.Comparer
{
    public sealed class ValidationErrorsModelComparer : IComparer<ValidationErrorDto>
    {
        public int Compare(ValidationErrorDto x, ValidationErrorDto y)
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
                return 1;
            }

            if (!IsError(x) && IsError(y))
            {
                return -1;
            }

            return string.CompareOrdinal(x.RuleName, y.RuleName);

            //if ((IsError(x) && IsError(y)) || (IsWarning(x) && IsWarning(y)))
            //{

            //}
        }

        private bool IsError(ValidationErrorDto x)
        {
            return string.Equals(x.Severity, "E", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(x.Severity, "F", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsWarning(ValidationErrorDto x)
        {
            return string.Equals(x.Severity, "W", StringComparison.OrdinalIgnoreCase);
        }
    }
}
