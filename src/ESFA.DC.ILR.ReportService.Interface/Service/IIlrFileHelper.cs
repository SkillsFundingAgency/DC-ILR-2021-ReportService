using ESFA.DC.ILR.ReportService.Interface.Context;

namespace ESFA.DC.ILR.ReportService.Interface.Service
{
    public interface IIlrFileHelper
    {
        bool CheckIlrFileNameIsValid(IReportServiceContext reportServiceContext);
    }
}