﻿using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface IFM81TrailBlazerProviderService
    {
        Task<FM81Global> GetFM81Data(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
