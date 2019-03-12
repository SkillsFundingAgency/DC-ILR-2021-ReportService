using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsCoInvestment;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsMonthlyPayment;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;
using LearningDeliveryInfo = ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsCoInvestment.LearningDeliveryInfo;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class IlrProviderService : IIlrProviderService
    {
        private const int ApprentishipsFundModel = 36;
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _storage;
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly DataStoreConfiguration _dataStoreConfiguration;
        private readonly SemaphoreSlim _getIlrLock;
        private Message _message;

        public IlrProviderService(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IXmlSerializationService xmlSerializationService,
            IDateTimeProvider dateTimeProvider,
            IIntUtilitiesService intUtilitiesService,
            DataStoreConfiguration dataStoreConfiguration)
        {
            _logger = logger;
            _storage = storage;
            _xmlSerializationService = xmlSerializationService;
            _dateTimeProvider = dateTimeProvider;
            _intUtilitiesService = intUtilitiesService;
            _dataStoreConfiguration = dataStoreConfiguration;
            _message = null;
            _getIlrLock = new SemaphoreSlim(1, 1);
        }

        public async Task<IMessage> GetIlrFile(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            await _getIlrLock.WaitAsync(cancellationToken);

            try
            {
                if (_message != null)
                {
                    return _message;
                }

                cancellationToken.ThrowIfCancellationRequested();

                string filename = reportServiceContext.Filename;
                int ukPrn = reportServiceContext.Ukprn;
                if (string.Equals(reportServiceContext.CollectionName, "ILR1819", StringComparison.OrdinalIgnoreCase))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await _storage.GetAsync(filename, ms, cancellationToken);
                        ms.Seek(0, SeekOrigin.Begin);
                        _message = _xmlSerializationService.Deserialize<Message>(ms);
                    }
                }
                else
                {
                    DateTime submittedDate;
                    DateTime filePreparationDate;

                    DbContextOptions<ILR1819_DataStoreEntities> options = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>().UseSqlServer(_dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
                    using (var ilrContext = new ILR1819_DataStoreEntities(options))
                    {
                        submittedDate = ilrContext.FileDetails.SingleOrDefault(x => x.Ukprn == ukPrn)?.SubmittedTime ?? _dateTimeProvider.ConvertUtcToUk(_dateTimeProvider.GetNowUtc());
                    }

                    DbContextOptions<ILR1819_DataStoreEntitiesValid> validContextOptions = new DbContextOptionsBuilder<ILR1819_DataStoreEntitiesValid>().UseSqlServer(_dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
                    using (var ilrContext = new ILR1819_DataStoreEntitiesValid(validContextOptions))
                    {
                        filePreparationDate = ilrContext.SourceFiles.SingleOrDefault(x => x.Ukprn == ukPrn)?.FilePreparationDate ?? _dateTimeProvider.ConvertUtcToUk(_dateTimeProvider.GetNowUtc());
                    }

                    _message = new Message
                    {
                        Header = new MessageHeader
                        {
                            Source = new MessageHeaderSource
                            {
                                UKPRN = ukPrn,
                                DateTime = submittedDate
                            },
                            CollectionDetails = new MessageHeaderCollectionDetails
                            {
                                FilePreparationDate = filePreparationDate
                            }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get and deserialise ILR from storage, key: {reportServiceContext.Filename}", ex);
            }
            finally
            {
                _getIlrLock.Release();
            }

            return _message;
        }

        public async Task<ILRSourceFileInfo> GetLastSubmittedIlrFile(
           IReportServiceContext reportServiceContext,
           CancellationToken cancellationToken)
        {
            var ilrFileDetail = new ILRSourceFileInfo();

            cancellationToken.ThrowIfCancellationRequested();

            var ukPrn = reportServiceContext.Ukprn;
            DbContextOptions<ILR1819_DataStoreEntities> options = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>().UseSqlServer(_dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
            using (var ilrContext = new ILR1819_DataStoreEntities(options))
            {
                var fileDetail = await ilrContext.FileDetails.Where(x => x.Ukprn == ukPrn).OrderByDescending(x => x.Id).FirstOrDefaultAsync(cancellationToken);
                if (fileDetail != null)
                {
                    var filename = fileDetail.Filename.Contains('/') ? fileDetail.Filename.Split('/')[1] : fileDetail.Filename;

                    ilrFileDetail.UKPRN = fileDetail.Ukprn;
                    ilrFileDetail.Filename = filename;
                    ilrFileDetail.SubmittedTime = fileDetail.SubmittedTime;
                }
            }

            DbContextOptions<ILR1819_DataStoreEntitiesValid> validContextOptions = new DbContextOptionsBuilder<ILR1819_DataStoreEntitiesValid>().UseSqlServer(_dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
            using (var ilrContext = new ILR1819_DataStoreEntitiesValid(validContextOptions))
            {
                var collectionDetail = await ilrContext.CollectionDetails.FirstOrDefaultAsync(x => x.Ukprn == ukPrn, cancellationToken);
                if (collectionDetail != null)
                {
                    ilrFileDetail.FilePreparationDate = collectionDetail.FilePreparationDate;
                }
            }

            return ilrFileDetail;
        }

        public async Task<AppsCoInvestmentILRInfo> GetILRInfoForAppsCoInvestmentReportAsync(int ukPrn, CancellationToken cancellationToken)
        {
            var appsCoInvestmentIlrInfo = new AppsCoInvestmentILRInfo
            {
                UkPrn = ukPrn,
                Learners = new List<LearnerInfo>()
            };

            cancellationToken.ThrowIfCancellationRequested();

            List<Learner> learnersList;
            DbContextOptions<ILR1819_DataStoreEntitiesValid> options = new DbContextOptionsBuilder<ILR1819_DataStoreEntitiesValid>().UseSqlServer(_dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
            using (var ilrContext = new ILR1819_DataStoreEntitiesValid(options))
            {
                learnersList = await ilrContext.Learners
                                                .Include(x => x.LearningDeliveries).ThenInclude(y => y.AppFinRecords)
                                                .Include(x => x.LearnerEmploymentStatuses)
                                                .Where(x => x.Ukprn == ukPrn && x.LearningDeliveries.Any(y => y.FundModel == ApprentishipsFundModel))
                                                .ToListAsync(cancellationToken);
            }

            foreach (var learner in learnersList)
            {
                var learnerInfo = new LearnerInfo
                {
                    LearnRefNumber = learner.LearnRefNumber,
                    LearningDeliveries = learner.LearningDeliveries.Select(x => new LearningDeliveryInfo()
                    {
                        UKPRN = ukPrn,
                        LearnRefNumber = x.LearnRefNumber,
                        LearnAimRef = x.LearnAimRef,
                        AimType = x.AimType,
                        AimSeqNumber = x.AimSeqNumber,
                        LearnStartDate = x.LearnStartDate,
                        ProgType = x.ProgType,
                        StdCode = x.StdCode,
                        FworkCode = x.FworkCode,
                        PwayCode = x.PwayCode,
                        SWSupAimId = x.SwsupAimId,
                        AppFinRecords = x.AppFinRecords.Select(y => new AppFinRecordInfo()
                        {
                            LearnRefNumber = y.LearnRefNumber,
                            AimSeqNumber = y.AimSeqNumber,
                            AFinType = y.AfinType,
                            AFinCode = y.AfinCode,
                            AFinDate = y.AfinDate,
                            AFinAmount = y.AfinAmount
                        }).ToList()
                    }).ToList(),
                    LearnerEmploymentStatus = learner.LearnerEmploymentStatuses.Select(x => new LearnerEmploymentStatusInfo()
                    {
                        LearnRefNumber = x.LearnRefNumber,
                        DateEmpStatApp = x.DateEmpStatApp,
                        EmpId = x.EmpId
                    }).ToList()
                };
                appsCoInvestmentIlrInfo.Learners.Add(learnerInfo);
            }

            return appsCoInvestmentIlrInfo;
        }

        public async Task<AppsAdditionalPaymentILRInfo> GetILRInfoForAppsAdditionalPaymentsReportAsync(int ukPrn, CancellationToken cancellationToken)
        {
            var appsAdditionalPaymentIlrInfo = new AppsAdditionalPaymentILRInfo()
            {
                UkPrn = ukPrn,
                Learners = new List<AppsAdditionalPaymentLearnerInfo>()
            };

            cancellationToken.ThrowIfCancellationRequested();

            List<Learner> learnersList;
            DbContextOptions<ILR1819_DataStoreEntitiesValid> options = new DbContextOptionsBuilder<ILR1819_DataStoreEntitiesValid>().UseSqlServer(_dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
            using (var ilrContext = new ILR1819_DataStoreEntitiesValid(options))
            {
                learnersList = await ilrContext.Learners
                                                .Where(x => x.Ukprn == ukPrn && x.LearningDeliveries.Any(y => y.FundModel == ApprentishipsFundModel))
                                                .ToListAsync(cancellationToken);
            }

            foreach (var learner in learnersList)
            {
                var learnerInfo = new AppsAdditionalPaymentLearnerInfo
                {
                    LearnRefNumber = learner.LearnRefNumber,
                };
                appsAdditionalPaymentIlrInfo.Learners.Add(learnerInfo);
            }

            return appsAdditionalPaymentIlrInfo;
        }

        public async Task<AppsMonthlyPaymentILRInfo> GetILRInfoForAppsMonthlyPaymentReportAsync(int ukPrn, CancellationToken cancellationToken)
        {
            var appsMonthlyPaymentIlrInfo = new AppsMonthlyPaymentILRInfo()
            {
                UkPrn = ukPrn,
                Learners = new List<AppsMonthlyPaymentLearnerInfo>()
            };

            cancellationToken.ThrowIfCancellationRequested();

            List<Learner> learnersList;
            DbContextOptions<ILR1819_DataStoreEntitiesValid> options = new DbContextOptionsBuilder<ILR1819_DataStoreEntitiesValid>().UseSqlServer(_dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
            using (var ilrContext = new ILR1819_DataStoreEntitiesValid(options))
            {
                learnersList = await ilrContext.Learners
                                                    .Include(x => x.LearningDeliveries).ThenInclude(y => y.LearningDeliveryFams)
                                                    .Include(x => x.LearningDeliveries).ThenInclude(y => y.ProviderSpecDeliveryMonitorings)
                                                    .Include(x => x.ProviderSpecLearnerMonitorings)
                                                    .Where(x => x.Ukprn == ukPrn && x.LearningDeliveries.Any(y => y.FundModel == ApprentishipsFundModel))
                                                    .ToListAsync(cancellationToken);
            }

            foreach (var learner in learnersList)
            {
                var learnerInfo = new AppsMonthlyPaymentLearnerInfo
                {
                    LearnRefNumber = learner.LearnRefNumber,
                    CampId = learner.CampId,
                    LearningDeliveries = learner.LearningDeliveries.Select(x => new AppsMonthlyPaymentLearningDeliveryInfo()
                    {
                        UKPRN = ukPrn,
                        LearnRefNumber = x.LearnRefNumber,
                        LearnAimRef = x.LearnAimRef,
                        AimType = x.AimType,
                        SWSupAimId = x.SwsupAimId,
                        ProviderSpecDeliveryMonitorings = x.ProviderSpecDeliveryMonitorings.Select(y => new AppsMonthlyPaymentProviderSpecDeliveryMonitoringInfo()
                        {
                            UKPRN = y.Ukprn,
                            LearnRefNumber = y.LearnRefNumber,
                            AimSeqNumber = y.AimSeqNumber,
                            ProvSpecDelMon = y.ProvSpecDelMon,
                            ProvSpecDelMonOccur = y.ProvSpecDelMonOccur
                        }).ToList(),
                        LearningDeliveryFams = x.LearningDeliveryFams.Select(y => new AppsMonthlyPaymentLearningDeliveryFAMInfo()
                        {
                            UKPRN = y.Ukprn,
                            LearnRefNumber = y.LearnRefNumber,
                            AimSeqNumber = y.AimSeqNumber,
                            LearnDelFAMType = y.LearnDelFamtype,
                            LearnDelFAMCode = y.LearnDelFamcode
                        }).ToList()
                    }).ToList(),
                    ProviderSpecLearnerMonitorings = learner.ProviderSpecLearnerMonitorings.Select(x => new AppsMonthlyPaymentProviderSpecLearnerMonitoringInfo()
                    {
                        UKPRN = x.Ukprn,
                        LearnRefNumber = x.LearnRefNumber,
                        ProvSpecLearnMon = x.ProvSpecLearnMon,
                        ProvSpecLearnMonOccur = x.ProvSpecLearnMonOccur
                    }).ToList()
                };

                appsMonthlyPaymentIlrInfo.Learners.Add(learnerInfo);
            }

            return appsMonthlyPaymentIlrInfo;
        }
    }
}