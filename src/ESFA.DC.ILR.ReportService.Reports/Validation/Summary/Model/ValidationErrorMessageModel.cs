namespace ESFA.DC.ILR.ReportService.Reports.Validation.Summary.Model
{
    public class ValidationErrorMessageModel
    {
        public string RuleName { get; set; }
        public string Message { get; set; }
        public int Occurrences { get; set; }
    }
}
