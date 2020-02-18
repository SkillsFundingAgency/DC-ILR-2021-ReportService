using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM06;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public class FrmLearnerComparer : IEqualityComparer<FrmLearnerKey>
    {
        public bool Equals(FrmLearnerKey x, FrmLearnerKey y)
        {
            return x?.GetHashCode() == y?.GetHashCode();
        }

        public int GetHashCode(FrmLearnerKey obj)
        {
            return new
            {
                LearnRefNumber = obj.LearnRefNumber.ToLowerInvariant(),
                FworkCodeNullable = obj.FworkCodeNullable,
                LearnAimRef = obj.LearnAimRef.ToLowerInvariant(),
                LearnStartDate = obj.LearnStartDate,
                ProgTypeNullable = obj.ProgTypeNullable,
                StdCodeNullable = obj.StdCodeNullable
            }.GetHashCode();
        }
    }
}
