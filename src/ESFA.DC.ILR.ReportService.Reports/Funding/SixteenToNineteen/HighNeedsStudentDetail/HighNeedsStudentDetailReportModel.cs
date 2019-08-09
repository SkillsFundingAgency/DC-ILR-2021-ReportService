namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentDetail
{
    public class HighNeedsStudentDetailReportModel : AbstractSixteenToNineteenModel
    {
        public bool StudentsWithAnEhcp { get; set; }

        public bool StudentsWithoutAnEhcp { get; set; }

        public bool HighNeedsStudentsWithoutAnEhcp { get; set; }

        public bool StudentsWithAnEhcpAndHns { get; set; }

        public bool StudentWithAnEhcpAndNotHns { get; set; }
    }
}
