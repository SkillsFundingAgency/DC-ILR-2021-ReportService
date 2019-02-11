using ESFA.DC.ILR1819.ReportService.Interface.Context;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IIlrFileHelper
    {
        bool CheckIlrFileNameIsValid(IReportServiceContext reportServiceContext);
    }
}