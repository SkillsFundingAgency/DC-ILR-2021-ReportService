using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            int cmp = string.CompareOrdinal(x.LearnRefNumber, y.LearnRefNumber);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = string.CompareOrdinal(x.ConditionOfFundingEnglish, y.ConditionOfFundingEnglish);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = string.CompareOrdinal(x.ConditionOfFundingMaths, y.ConditionOfFundingMaths);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = string.CompareOrdinal(x.ProvSpecLearnMonA, y.ProvSpecLearnMonA);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = string.CompareOrdinal(x.ProvSpecLearnMonB, y.ProvSpecLearnMonB);
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
