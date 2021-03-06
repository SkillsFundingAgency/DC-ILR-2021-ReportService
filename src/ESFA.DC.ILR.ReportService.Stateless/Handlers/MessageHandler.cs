﻿using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Modules;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Stateless.Configuration;
using ESFA.DC.ILR.ReportService.Stateless.Context;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ServiceFabric.Common.Config;
using ESFA.DC.ServiceFabric.Common.Config.Interface;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.ILR.ReportService.Stateless.Handlers
{
    public sealed class MessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly ILifetimeScope _parentLifeTimeScope;
        private readonly StatelessServiceContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler"/> class.
        /// Simple constructor for use by AutoFac testing, don't want to have to fake a @see StatelessServiceContext.
        /// </summary>
        /// <param name="parentLifeTimeScope">AutoFac scope.</param>
        /// <param name="jobContextMessageKeysMutator">jobContextMessageKeysMutator.</param>
        public MessageHandler(ILifetimeScope parentLifeTimeScope)
        {
            _parentLifeTimeScope = parentLifeTimeScope;
            _context = null;
        }

        public MessageHandler(ILifetimeScope parentLifeTimeScope, StatelessServiceContext context)
        {
            _parentLifeTimeScope = parentLifeTimeScope;
            _context = context;
        }

        public async Task<bool> HandleAsync(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            try
            {
                using (var childLifeTimeScope = GetChildLifeTimeScope(jobContextMessage))
                {
                    var executionContext = (ExecutionContext)childLifeTimeScope.Resolve<IExecutionContext>();
                    var versionInfo = childLifeTimeScope.Resolve<IVersionInfo>();

                    executionContext.JobId = jobContextMessage.JobId.ToString();
                    var logger = childLifeTimeScope.Resolve<ILogger>();
                    logger.LogDebug("Started Report Service");

                    var entryPoint = childLifeTimeScope.Resolve<IEntryPoint>();

                    var result = await entryPoint.Callback(new ReportServiceJobContextMessageContext(jobContextMessage, versionInfo), cancellationToken);

                    logger.LogDebug($"Completed Report Service");
                    return true;
                }
            }
            catch (OutOfMemoryException oom)
            {
                Environment.FailFast("Report Service Out Of Memory", oom);
                throw;
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(_context, "Exception-{0}", ex.ToString());
                throw;
            }
        }

        public ILifetimeScope GetChildLifeTimeScope(JobContextMessage jobContextMessage)
        {
            return _parentLifeTimeScope.BeginLifetimeScope(c =>
            {
                c.RegisterInstance(jobContextMessage).As<IJobContextMessage>();

                var azureBlobStorageOptions = _parentLifeTimeScope.Resolve<IAzureStorageOptions>();
                c.RegisterInstance(new AzureStorageKeyValuePersistenceConfig(
                        azureBlobStorageOptions.AzureBlobConnectionString,
                        jobContextMessage.KeyValuePairs[ILRContextKeys.Container].ToString()))
                    .As<IAzureStorageKeyValuePersistenceServiceConfig>();

                switch (jobContextMessage.KeyValuePairs["CollectionName"].ToString())
                {
                    case CollectionConstants.IlrCollectionName:
                        c.RegisterModule<DataModule>();
                        break;

                    case CollectionConstants.EasCollectionName:
                        IServiceFabricConfigurationService serviceFabricConfigurationService = new ServiceFabricConfigurationService();
                        var databaseConfiguration = serviceFabricConfigurationService.GetConfigSectionAs<DatabaseConfiguration>("DatabaseConfiguration");

                        c.RegisterModule(new EasDataModule(databaseConfiguration));
                        break;
                }
            });
        }
    }
}
