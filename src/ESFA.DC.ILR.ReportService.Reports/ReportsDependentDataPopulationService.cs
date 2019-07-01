using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;
using ESFA.DC.ILR.ReportService.Service.Model;

namespace ESFA.DC.ILR.ReportService.Reports
{
    public class ReportsDependentDataPopulationService : IReportsDependentDataPopulationService
    {
        private readonly IIndex<Type, IExternalDataProvider> _providers;

        public ReportsDependentDataPopulationService(IIndex<Type, IExternalDataProvider> providers)
        {
            _providers = providers;
           
        }

        public async Task<ReportServiceDependentData> PopulateAsync(
            IReportServiceContext reportServiceContext,
            IEnumerable<Type> dependentTypes,
            CancellationToken cancellationToken)
        {
            ReportServiceDependentData rsDependentData = new ReportServiceDependentData
            {
                Data = new Dictionary<Type, object>()
            };
            foreach (var type in dependentTypes)
            {
                rsDependentData.Data[type] = await _providers[type].ProvideAsync(reportServiceContext, cancellationToken);
            }

            return rsDependentData;
        }
    }
}
