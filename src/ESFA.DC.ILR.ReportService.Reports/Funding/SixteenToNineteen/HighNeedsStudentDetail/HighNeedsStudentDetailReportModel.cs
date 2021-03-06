﻿using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentDetail
{
    public class HighNeedsStudentDetailReportModel : AbstractSixteenToNineteenModel
    {
        public string DerivedFundline { get; set; }

        public bool StudentsWithAnEhcp { get; set; }

        public bool StudentsWithoutAnEhcp { get; set; }

        public bool HighNeedsStudentsWithoutAnEhcp { get; set; }

        public bool StudentsWithAnEhcpAndHns { get; set; }

        public bool StudentWithAnEhcpAndNotHns { get; set; }
    }
}
