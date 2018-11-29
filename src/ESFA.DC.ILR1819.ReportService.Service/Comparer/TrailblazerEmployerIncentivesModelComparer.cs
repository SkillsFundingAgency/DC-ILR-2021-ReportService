using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Comparer
{
    public sealed class TrailblazerEmployerIncentivesModelComparer : IComparer<TrailblazerEmployerIncentivesModel>
    {
        public int Compare(TrailblazerEmployerIncentivesModel x, TrailblazerEmployerIncentivesModel y)
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

            if (x.EmployerIdentifier < y.EmployerIdentifier)
            {
                return -1;
            }

            if (y.EmployerIdentifier < x.EmployerIdentifier)
            {
                return 1;
            }

            if (x.EmployerIdentifier == y.EmployerIdentifier)
            {
                return 0;
            }

            return 0;
        }
    }
}