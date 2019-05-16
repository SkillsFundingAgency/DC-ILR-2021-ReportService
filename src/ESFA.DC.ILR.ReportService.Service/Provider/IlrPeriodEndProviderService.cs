using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsCoInvestment;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsMonthlyPayment;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.ILRDataQuality;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.ILR1819.DataStore.EF.Valid.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using LearningDeliveryInfo = ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsCoInvestment.LearningDeliveryInfo;

namespace ESFA.DC.ILR.ReportService.Service.Provider
{
    public sealed class IlrPeriodEndProviderService : IIlrPeriodEndProviderService
    {
        private const int ApprentishipsFundModel = 36;
        private readonly Func<IIlr1819ValidContext> _ilrValidContextFactory;
        private readonly Func<IIlr1819RulebaseContext> _ilrContextFactory;

        public IlrPeriodEndProviderService(
            ILogger logger,
            Func<IIlr1819ValidContext> ilrValidContextFactory,
            Func<IIlr1819RulebaseContext> ilrContextFactory)
        {
            _ilrValidContextFactory = ilrValidContextFactory;
            _ilrContextFactory = ilrContextFactory;
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
            using (var ilrContext = _ilrValidContextFactory())
            {
                learnersList = await ilrContext.Learners
                                                .Include(x => x.LearningDeliveries).ThenInclude(y => y.AppFinRecords)
                                                .Include(x => x.LearnerEmploymentStatuses)
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
            using (var ilrContext = _ilrValidContextFactory())
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
                    ULN = learner.ULN,
                    LearningDeliveries = learner.LearningDeliveries.Select(x => new AppsAdditionalPaymentLearningDeliveryInfo()
                    {
                        UKPRN = ukPrn,
                        LearnRefNumber = x.LearnRefNumber,
                        LearnAimRef = x.LearnAimRef,
                        AimType = x.AimType,
                        LearnStartDate = x.LearnStartDate,
                        ProgType = x.ProgType,
                        StdCode = x.StdCode,
                        FworkCode = x.FworkCode,
                        PwayCode = x.PwayCode,
                        AimSeqNumber = x.AimSeqNumber,
                        FundModel = x.FundModel
                    }).ToList(),
                    ProviderSpecLearnerMonitorings = learner.ProviderSpecLearnerMonitorings.Select(x => new AppsAdditionalPaymentProviderSpecLearnerMonitoringInfo()
                    {
                        UKPRN = x.UKPRN,
                        LearnRefNumber = x.LearnRefNumber,
                        ProvSpecLearnMon = x.ProvSpecLearnMon,
                        ProvSpecLearnMonOccur = x.ProvSpecLearnMonOccur
                    }).ToList()
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
            using (var ilrContext = _ilrValidContextFactory())
            {
                learnersList = await ilrContext.Learners
                                                    .Include(x => x.LearningDeliveries).ThenInclude(y => y.LearningDeliveryFAMs)
                                                    .Include(x => x.LearningDeliveries).ThenInclude(y => y.ProviderSpecDeliveryMonitorings)
                                                    .Include(x => x.ProviderSpecLearnerMonitorings)
                                                    .Where(x => x.UKPRN == ukPrn && x.LearningDeliveries.Any(y => y.FundModel == ApprentishipsFundModel))
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
                        SWSupAimId = x.SWSupAimId,
                        LearnStartDate = x.LearnStartDate,
                        ProgType = x.ProgType,
                        StdCode = x.StdCode,
                        FworkCode = x.FworkCode,
                        PwayCode = x.PwayCode,
                        AimSeqNumber = x.AimSeqNumber,
                        EPAOrganisation = x.EPAOrgID,
                        PartnerUkPrn = x.PartnerUKPRN,
                        ProviderSpecDeliveryMonitorings = x.ProviderSpecDeliveryMonitorings.Select(y => new AppsMonthlyPaymentProviderSpecDeliveryMonitoringInfo()
                        {
                            UKPRN = y.UKPRN,
                            LearnRefNumber = y.LearnRefNumber,
                            AimSeqNumber = y.AimSeqNumber,
                            ProvSpecDelMon = y.ProvSpecDelMon,
                            ProvSpecDelMonOccur = y.ProvSpecDelMonOccur
                        }).ToList(),
                        LearningDeliveryFams = x.LearningDeliveryFAMs.Select(y => new AppsMonthlyPaymentLearningDeliveryFAMInfo()
                        {
                            UKPRN = y.UKPRN,
                            LearnRefNumber = y.LearnRefNumber,
                            AimSeqNumber = y.AimSeqNumber,
                            LearnDelFAMType = y.LearnDelFAMType,
                            LearnDelFAMCode = y.LearnDelFAMCode
                        }).ToList()
                    }).ToList(),
                    ProviderSpecLearnerMonitorings = learner.ProviderSpecLearnerMonitorings.Select(x => new AppsMonthlyPaymentProviderSpecLearnerMonitoringInfo()
                    {
                        UKPRN = x.UKPRN,
                        LearnRefNumber = x.LearnRefNumber,
                        ProvSpecLearnMon = x.ProvSpecLearnMon,
                        ProvSpecLearnMonOccur = x.ProvSpecLearnMonOccur
                    }).ToList()
                };

                appsMonthlyPaymentIlrInfo.Learners.Add(learnerInfo);
            }

            return appsMonthlyPaymentIlrInfo;
        }

        public async Task<List<RuleViolationsInfo>> GetTop20RuleViolationsAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<RuleViolationsInfo> top20RuleViolationsList;
            using (var ilrContext = _ilrContextFactory())
            {
                top20RuleViolationsList = await ilrContext.ValidationErrors
                    .Where(x => x.Severity == "E")
                    .GroupBy(x => new { x.RuleName, x.ErrorMessage })
                    .Select(x => new RuleViolationsInfo
                    {
                        RuleName = x.Key.RuleName,
                        ErrorMessage = x.Key.ErrorMessage,
                        Providers = x.Select(y => y.UKPRN).Distinct().Count(),
                        Learners = x.Select(y => y.LearnRefNumber).Distinct().Count(),
                        NoOfErrors = x.Select(y => y.ErrorMessage).Count()
                    })
                    .OrderByDescending(x => x.NoOfErrors)
                    .ThenBy(x => x.RuleName)
                    .ThenByDescending(x => x.Providers)
                    .Take(20)
                    .ToListAsync(cancellationToken);
            }

          return top20RuleViolationsList;
        }
    }
}