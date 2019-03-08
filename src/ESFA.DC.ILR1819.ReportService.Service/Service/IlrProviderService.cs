﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
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

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

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

                    using (var ilrContext = new ILR1819_DataStoreEntities(_dataStoreConfiguration.ILRDataStoreConnectionString))
                    {
                        submittedDate = ilrContext.FileDetails.SingleOrDefault(x => x.UKPRN == ukPrn)?.SubmittedTime ?? _dateTimeProvider.ConvertUtcToUk(_dateTimeProvider.GetNowUtc());
                    }

                    using (var ilrContext = new ILR1819_DataStoreEntitiesValid(_dataStoreConfiguration.ILRDataStoreConnectionString))
                    {
                        filePreparationDate = ilrContext.SourceFiles.SingleOrDefault(x => x.UKPRN == ukPrn)?.FilePreparationDate ?? _dateTimeProvider.ConvertUtcToUk(_dateTimeProvider.GetNowUtc());
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
            await _getIlrLock.WaitAsync(cancellationToken);
            var ilrFileDetail = new ILRSourceFileInfo();
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                var ukPrn = reportServiceContext.Ukprn;

                using (var ilrContext = new ILR1819_DataStoreEntities(_dataStoreConfiguration.ILRDataStoreConnectionString))
                {
                    var fileDetail = await ilrContext.FileDetails.Where(x => x.UKPRN == ukPrn).OrderByDescending(x => x.ID).FirstOrDefaultAsync(cancellationToken);
                    if (fileDetail != null)
                    {
                        var filename = fileDetail.Filename.Contains('/') ? fileDetail.Filename.Split('/')[1] : fileDetail.Filename;

                        ilrFileDetail.UKPRN = fileDetail.UKPRN;
                        ilrFileDetail.Filename = filename;
                        ilrFileDetail.SubmittedTime = fileDetail.SubmittedTime;
                    }
                }

                using (var ilrContext = new ILR1819_DataStoreEntitiesValid(_dataStoreConfiguration.ILRDataStoreValidConnectionString))
                {
                    var collectionDetail = await ilrContext.CollectionDetails.FirstOrDefaultAsync(x => x.UKPRN == ukPrn, cancellationToken);
                    if (collectionDetail != null)
                    {
                        ilrFileDetail.FilePreparationDate = collectionDetail.FilePreparationDate;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get Last Submitted ILR file details ", ex);
            }
            finally
            {
                _getIlrLock.Release();
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

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                List<Learner> learnersList;
                using (var ilrContext = new ILR1819_DataStoreEntitiesValid(_dataStoreConfiguration.ILRDataStoreValidConnectionString))
                {
                    learnersList = await ilrContext.Learners
                                                    .Include(x => x.LearningDeliveries.Select(y => y.AppFinRecords))
                                                    .Include(x => x.LearnerEmploymentStatus)
                                                    .Where(x => x.UKPRN == ukPrn && x.LearningDeliveries.Any(y => y.FundModel == ApprentishipsFundModel))
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
                            SWSupAimId = x.SWSupAimId,
                            AppFinRecords = x.AppFinRecords.Select(y => new AppFinRecordInfo()
                            {
                                LearnRefNumber = y.LearnRefNumber,
                                AimSeqNumber = y.AimSeqNumber,
                                AFinType = y.AFinType,
                                AFinCode = y.AFinCode,
                                AFinDate = y.AFinDate,
                                AFinAmount = y.AFinAmount
                            }).ToList()
                        }).ToList(),
                        LearnerEmploymentStatus = learner.LearnerEmploymentStatus.Select(x => new LearnerEmploymentStatusInfo()
                        {
                            LearnRefNumber = x.LearnRefNumber,
                            DateEmpStatApp = x.DateEmpStatApp,
                            EmpId = x.EmpId
                        }).ToList()
                    };
                    appsCoInvestmentIlrInfo.Learners.Add(learnerInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get ILR Details - AppsCoInvestmentContributions  ", ex);
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
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                List<Learner> learnersList;
                using (var ilrContext = new ILR1819_DataStoreEntitiesValid(_dataStoreConfiguration.ILRDataStoreValidConnectionString))
                {
                    learnersList = await ilrContext.Learners
                                                    .Where(x => x.UKPRN == ukPrn && x.LearningDeliveries.Any(y => y.FundModel == ApprentishipsFundModel))
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
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get ILR Details - AppsAdditionalPaymentILRInfo  ", ex);
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
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                List<Learner> learnersList;
                using (var ilrContext = new ILR1819_DataStoreEntitiesValid(_dataStoreConfiguration.ILRDataStoreValidConnectionString))
                {
                    learnersList = await ilrContext.Learners
                        .Where(x => x.UKPRN == ukPrn && x.LearningDeliveries.Any(y => y.FundModel == ApprentishipsFundModel))
                        .ToListAsync(cancellationToken);
                }

                foreach (var learner in learnersList)
                {
                    var learnerInfo = new AppsMonthlyPaymentLearnerInfo
                    {
                        LearnRefNumber = learner.LearnRefNumber
                    };
                    appsMonthlyPaymentIlrInfo.Learners.Add(learnerInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get ILR Details - AppsMonthlyPaymentILRInfo  ", ex);
            }

            return appsMonthlyPaymentIlrInfo;
        }
    }
}