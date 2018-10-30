using System;
using System.Collections.Generic;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Comparer
{
    public sealed class MathsAndEnglishModelComparer : IComparer<MathsAndEnglishModel>
    {
        public int Compare(MathsAndEnglishModel x, MathsAndEnglishModel y)
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

            int cmp = string.Compare(x.LearnRefNumber, y.LearnRefNumber, StringComparison.OrdinalIgnoreCase);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = string.Compare(x.ConditionOfFundingEnglish, y.ConditionOfFundingEnglish, StringComparison.OrdinalIgnoreCase);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = string.Compare(x.ConditionOfFundingMaths, y.ConditionOfFundingMaths, StringComparison.OrdinalIgnoreCase);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = string.Compare(x.ProvSpecLearnMonA, y.ProvSpecLearnMonA, StringComparison.OrdinalIgnoreCase);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = string.Compare(x.ProvSpecLearnMonB, y.ProvSpecLearnMonB, StringComparison.OrdinalIgnoreCase);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = Nullable.Compare(x.LearnerStartDate, y.LearnerStartDate);
            if (cmp < 0)
            {
                return -1;
            }

            if (cmp > 0)
            {
                return 1;
            }

            return 0;
        }
    }
}
