using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop
{
    public class ReportServiceDesktopTask: IDesktopTask
    {
        private ILifetimeScope _lifeTimeScope;
        private readonly IReportServiceContextFactory<IDesktopContext> _reportServiceContextFactory;

        public ReportServiceDesktopTask(ILifetimeScope lifeTimeScope, IReportServiceContextFactory<IDesktopContext> reportServiceContextFactory)
        {
            _lifeTimeScope = lifeTimeScope;
            _reportServiceContextFactory = reportServiceContextFactory;
        }

        public async Task<IDesktopContext> ExecuteAsync(IDesktopContext desktopContext, CancellationToken cancellationToken)
        {
            var reportServiceContext = _reportServiceContextFactory.Build(desktopContext);

            using (var childLifetimeScope = _lifeTimeScope.BeginLifetimeScope())
            {
                var entryPoint = childLifetimeScope.Resolve<IEntryPoint>();

                await entryPoint.Callback(reportServiceContext, cancellationToken);
            }

            return desktopContext;
        }
    }
}
