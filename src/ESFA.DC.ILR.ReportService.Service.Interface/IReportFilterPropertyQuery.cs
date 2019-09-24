namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReportFilterPropertyQuery
    {
        string PropertyName { get; }

        object Value { get; }
    }
}
