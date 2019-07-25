using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IFileNameService
    {
        string GetFilename(IReportServiceContext reportServiceContext, string fileName, OutputTypes outputType, bool includeDateTime = true);
    }
}
