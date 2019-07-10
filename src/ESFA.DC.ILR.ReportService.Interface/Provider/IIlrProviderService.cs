﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface IIlrProviderService
    {
        Task<IMessage> GetIlrFile(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);

        Task<NonContractedAppsActivityILRInfo> GetILRInfoForNonContractedAppsActivityReportAsync(List<string> validLearners, IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
