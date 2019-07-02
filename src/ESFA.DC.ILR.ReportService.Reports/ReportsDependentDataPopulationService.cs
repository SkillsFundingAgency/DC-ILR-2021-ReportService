using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Model;
using ESFA.DC.ILR.ReportService.Service.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports
{
    public class ReportsDependentDataPopulationService : IReportsDependentDataPopulationService
    {
        private readonly IIndex<Type, IExternalDataProvider> _providers;

        public ReportsDependentDataPopulationService(IIndex<Type, IExternalDataProvider> providers)
        {
            _providers = providers;
           
        }

        public async Task<IReportServiceDependentData> PopulateAsync(
            IReportServiceContext reportServiceContext,
            IEnumerable<Type> dependentTypes,
            CancellationToken cancellationToken)
        {
            var rsDependentData = new ReportServiceDependentData();

            foreach (Type type in dependentTypes)
            {
                var value = await _providers[type].ProvideAsync(reportServiceContext, cancellationToken);
                rsDependentData.Set(type, value);
            }

            return rsDependentData;
        }
    }
}
