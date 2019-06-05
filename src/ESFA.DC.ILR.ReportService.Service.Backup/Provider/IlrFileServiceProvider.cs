using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider
{
    public sealed class IlrFileServiceProvider : AbstractFundModelProviderService, IIlrProviderService
    {
        private readonly SemaphoreSlim _getIlrLock = new SemaphoreSlim(1, 1);
        private Message _message;

        public IlrFileServiceProvider(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IXmlSerializationService xmlSerializationService)
        : base(storage, xmlSerializationService, logger)
        {
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
                using (MemoryStream ms = new MemoryStream())
                {
                    await _streamableKeyValuePersistenceService.GetAsync(filename, ms, cancellationToken);
                    ms.Seek(0, SeekOrigin.Begin);
                    _message = _serializationService.Deserialize<Message>(ms);
                }
            }
            finally
            {
                _getIlrLock.Release();
            }

            return _message;
        }

        public async Task<NonContractedAppsActivityILRInfo> GetILRInfoForNonContractedAppsActivityReportAsync(List<string> validLearners, IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nonContractedAppsActivityIlrInfo = new NonContractedAppsActivityILRInfo()
            {
                UkPrn = reportServiceContext.Ukprn,
                Learners = new List<NonContractedAppsActivityLearnerInfo>()
            };

            var message = await GetIlrFile(reportServiceContext, cancellationToken);

            var learners = message.Learners?.Where(x => validLearners.Contains(x.LearnRefNumber)).ToArray();

            if (learners != null)
            {
                foreach (var learner in learners)
                {
                    var learnerInfo = new NonContractedAppsActivityLearnerInfo
                    {
                        LearnRefNumber = learner.LearnRefNumber,
                        UniqueLearnerNumber = learner.ULN,
                        DateOfBirth = learner.DateOfBirthNullable.GetValueOrDefault(),
                        CampId = learner.CampId,
                        LearningDeliveries = learner.LearningDeliveries.Select(x =>
                            new NonContractedAppsActivityLearningDeliveryInfo()
                            {
                                AimSeqNumber = x.AimSeqNumber,
                                LearnAimRef = x.LearnAimRef,
                                FundModel = x.FundModel,
                                UKPRN = reportServiceContext.Ukprn,
                                AimType = x.AimType,
                                SWSupAimId = x.SWSupAimId,
                                OriginalLearnStartDate = x.OrigLearnStartDateNullable,
                                LearnStartDate = x.LearnStartDate,
                                LearningPlannedEndDate = x.LearnPlanEndDate,
                                LearnActualEndDate = x.LearnActEndDateNullable,
                                ProgType = x.ProgTypeNullable,
                                StdCode = x.StdCodeNullable,
                                FworkCode = x.FworkCodeNullable,
                                PwayCode = x.PwayCodeNullable,
                                EPAOrganisation = x.EPAOrgID,
                                PartnerUkPrn = x.PartnerUKPRNNullable,
                                ProviderSpecDeliveryMonitorings = x.ProviderSpecDeliveryMonitorings.Select(y =>
                                    new NonContractedAppsActivityProviderSpecDeliveryMonitoringInfo()
                                    {
                                        ProvSpecDelMon = y.ProvSpecDelMon,
                                        ProvSpecDelMonOccur = y.ProvSpecDelMonOccur
                                    }).ToList(),
                                LearningDeliveryFams = x.LearningDeliveryFAMs.Select(y =>
                                    new NonContractedAppsActivityLearningDeliveryFAMInfo()
                                    {
                                        LearnDelFAMType = y.LearnDelFAMType,
                                        LearnDelFAMCode = y.LearnDelFAMCode,
                                        LearnDelFAMAppliesFrom = y.LearnDelFAMDateFromNullable,
                                        LearnDelFAMAppliesTo = y.LearnDelFAMDateToNullable
                                    }).ToList()
                            }).ToList(),
                        ProviderSpecLearnerMonitorings = learner.ProviderSpecLearnerMonitorings.Select(x =>
                            new NonContractedAppsActivityProviderSpecLearnerMonitoringInfo()
                            {
                                ProvSpecLearnMon = x.ProvSpecLearnMon,
                                ProvSpecLearnMonOccur = x.ProvSpecLearnMonOccur
                            }).ToList()
                    };

                    nonContractedAppsActivityIlrInfo.Learners.Add(learnerInfo);
                }
            }

            return nonContractedAppsActivityIlrInfo;
        }
    }
}