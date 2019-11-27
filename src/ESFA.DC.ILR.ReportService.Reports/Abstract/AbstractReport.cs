namespace ESFA.DC.ILR.ReportService.Reports.Abstract
{
    public abstract class AbstractReport
    {
        protected AbstractReport(string taskName, string fileName)
        {
            TaskName = taskName;
            ReportName = fileName;
        }

        public string TaskName { get; }

        public string ReportName { get; }
    }
}
