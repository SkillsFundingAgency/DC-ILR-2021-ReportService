namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData
{
    public class ValidationError
    {
        public string RuleName { get; set; }

        public ValidationError.SeverityLevel Severity { get; set; }

        public string Message { get; set; }

        public enum SeverityLevel
        {
            Error,
            Warning,
            Fail,
        }
    }
}
