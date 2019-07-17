namespace ESFA.DC.ILR.ReportService.Interface.Service
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.ILR.ReportService.Service.Interface;

    public interface IInvalidLearnersService
    {
        Task<List<string>> GetLearnersAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
