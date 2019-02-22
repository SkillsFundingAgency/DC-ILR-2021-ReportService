using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DASPayments.EF;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsCoInvestment;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public class DASPaymentsProviderService : IDASPaymentsProviderService
    {
        private const int FundingSource = 3;
        private int[] TransactionTypes = { 1, 2, 3 };
        private readonly ILogger _logger;
        private readonly DASPaymentsConfiguration _dasPaymentsConfiguration;
        private readonly SemaphoreSlim _getDasPaymentsLock;

        public DASPaymentsProviderService(ILogger logger, DASPaymentsConfiguration dasPaymentsConfiguration)
        {
            _logger = logger;
            _dasPaymentsConfiguration = dasPaymentsConfiguration;
            _getDasPaymentsLock = new SemaphoreSlim(1, 1);
        }

        public async Task<AppsCoInvestmentPaymentsInfo> GetPaymentsInfoForAppsCoInvestmentReportAsync(int ukPrn, CancellationToken cancellationToken)
        {
            await _getDasPaymentsLock.WaitAsync(cancellationToken);

            var appsCoInvestmentPaymentsInfo = new AppsCoInvestmentPaymentsInfo
            {
                UkPrn = ukPrn,
                Payments = new List<PaymentInfo>()
            };

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                List<Payment> paymentsList;
                DbContextOptions<DASPaymentsContext> options = new DbContextOptionsBuilder<DASPaymentsContext>().UseSqlServer(_dasPaymentsConfiguration.DASPaymentsConnectionString).Options;
                using (var context = new DASPaymentsContext(options))
                {
                    paymentsList = await context.Payments.Where(x => x.Ukprn == ukPrn &&
                                                                    x.FundingSource == FundingSource &&
                                                                    TransactionTypes.Contains(x.TransactionType)).ToListAsync(cancellationToken);
                }

                foreach (var payment in paymentsList)
                {
                    var paymentInfo = new PaymentInfo
                    {
                        FundingSource = payment.FundingSource,
                        TransactionType = payment.TransactionType,
                        AcademicYear = payment.AcademicYear,
                        CollectionPeriod = payment.CollectionPeriod,
                        ContractType = payment.ContractType,
                        DeliveryPeriod = payment.DeliveryPeriod,
                        LearnerReferenceNumber = payment.LearnerReferenceNumber,
                        LearnerUln = payment.LearnerUln,
                        LearningAimFrameworkCode = payment.LearningAimFrameworkCode,
                        LearningAimPathwayCode = payment.LearningAimPathwayCode,
                        LearningAimProgrammeType = payment.LearningAimProgrammeType,
                        LearningAimReference = payment.LearningAimReference,
                        LearningAimStandardCode = payment.LearningAimStandardCode
                    };

                    appsCoInvestmentPaymentsInfo.Payments.Add(paymentInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get DASPaymentsProviderService - AppsCoInvestmentContributions  ", ex);
            }
            finally
            {
                _getDasPaymentsLock.Release();
            }

            return appsCoInvestmentPaymentsInfo;
        }
    }
}
