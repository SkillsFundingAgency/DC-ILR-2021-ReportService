using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Models.Ilr;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR2021.DataStore.EF.Valid.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Providers
{
    public class ValidIlrProvider : IExternalDataProvider
    {
        private readonly Func<IILR2021_DataStoreEntitiesValid> _ilrContext;

        public ValidIlrProvider(Func<IILR2021_DataStoreEntitiesValid> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            using (var context = _ilrContext())
            {
                return new Message()
                {
                    HeaderEntity = new MessageHeader()
                    {
                        CollectionDetailsEntity = await context.CollectionDetails
                            .Where(x => x.UKPRN == reportServiceContext.Ukprn)
                            .OrderByDescending(d => d.FilePreparationDate)
                            .Select(cd => new MessageHeaderCollectionDetails()
                            {
                                FilePreparationDate = cd.FilePreparationDate.Value
                            }).FirstOrDefaultAsync(cancellationToken)
                    },
                    LearnersData = await context.Learners
                        .Where(x => x.UKPRN == reportServiceContext.Ukprn)
                        .Select(learner => new MessageLearner()
                        {
                            LearnRefNumber = learner.LearnRefNumber,
                            LearningDeliveriesData = learner.LearningDeliveries
                                .Select(learningDelivery => new MessageLearnerLearningDelivery()
                                {
                                    AimSeqNumber = learningDelivery.AimSeqNumber,
                                    LearningDeliveryFAMsData = learningDelivery.LearningDeliveryFAMs
                                        .Select(x => new MessageLearnerLearningDeliveryLearningDeliveryFAM()
                                        {
                                            LearnDelFAMType = x.LearnDelFAMType,
                                            LearnDelFAMCode = x.LearnDelFAMCode
                                        }).ToList()
                                }).ToList()
                        }).ToListAsync(cancellationToken)
                };
            }
        }
    }
}
