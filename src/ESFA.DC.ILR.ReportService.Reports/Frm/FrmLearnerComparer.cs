using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM06;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public class FrmLearnerComparer : IEqualityComparer<FrmLearnerKey>
    {
        public bool Equals(FrmLearnerKey x, FrmLearnerKey y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(FrmLearnerKey obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return (
                obj.LearnRefNumber = obj.LearnRefNumber.ToLowerInvariant(),
                obj.FworkCodeNullable = obj.FworkCodeNullable,
                obj.LearnAimRef = obj.LearnAimRef.ToLowerInvariant(),
                obj.LearnStartDate = obj.LearnStartDate,
                obj.ProgTypeNullable = obj.ProgTypeNullable,
                obj.StdCodeNullable = obj.StdCodeNullable
            ).GetHashCode();
        }
    }
}
