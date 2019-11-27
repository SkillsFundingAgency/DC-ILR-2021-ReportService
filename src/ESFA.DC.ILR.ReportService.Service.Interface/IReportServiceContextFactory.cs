namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReportServiceContextFactory<T>
    {
        IReportServiceContext Build(T Context);
    }
}
