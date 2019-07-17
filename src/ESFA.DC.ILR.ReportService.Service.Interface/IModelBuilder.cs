namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IModelBuilder<out T>
    {
        T Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData);
    }
}
