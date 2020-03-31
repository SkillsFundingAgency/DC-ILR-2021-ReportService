using System;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM06
{
    public class FrmLearnerKey
    {
        public string LearnRefNumber { get; set; }

        public string LearnAimRef { get; set; }

        public int? ProgTypeNullable { get; set; }

        public int? StdCodeNullable { get; set; }

        public int? FworkCodeNullable { get; set; }

        public DateTime LearnStartDate { get; set; }
    }
}
