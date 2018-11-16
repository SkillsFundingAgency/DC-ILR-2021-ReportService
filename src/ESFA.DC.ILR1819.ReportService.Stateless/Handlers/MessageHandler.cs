﻿using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR1819.ReportService.Service;
using ESFA.DC.ILR1819.ReportService.Stateless.Configuration;
using ESFA.DC.ILR1819.ReportService.Stateless.Interfaces;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Stateless.Handlers
{
    public sealed class MessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly ILifetimeScope _parentLifeTimeScope;
        private readonly StatelessServiceContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler"/> class.
        /// Simple constructor for use by AutoFac testing, don't want to have to fake a @see StatelessServiceContext
        /// </summary>
        /// <param name="parentLifeTimeScope">AutoFac scope</param>
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
                using (var childLifeTimeScope = _parentLifeTimeScope.BeginLifetimeScope(c =>
                {
                    c.RegisterInstance(jobContextMessage).As<IJobContextMessage>();

                    var azureBlobStorageOptions = _parentLifeTimeScope.Resolve<IAzureStorageOptions>();
                    c.RegisterInstance(new AzureStorageKeyValuePersistenceConfig(
                            azureBlobStorageOptions.AzureBlobConnectionString,
                            jobContextMessage.KeyValuePairs[JobContextMessageKey.Container].ToString()))
                        .As<IAzureStorageKeyValuePersistenceServiceConfig>();
                }))
                {
                    var executionContext = (Logging.ExecutionContext)childLifeTimeScope.Resolve<IExecutionContext>();
                    executionContext.JobId = jobContextMessage.JobId.ToString();
                    var logger = childLifeTimeScope.Resolve<ILogger>();
                    logger.LogDebug("Started Report Service");

                    var entryPoint = childLifeTimeScope.Resolve<EntryPoint>();
                    var result = false;
                    try
                    {
                        result = await entryPoint.Callback(jobContextMessage, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message, ex);
                    }

                    logger.LogDebug("Completed Report Service");
                    return result;
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(_context, "Exception-{0}", ex.ToString());

                throw;
            }
        }
    }
}
