using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM08
{
    public class Frm08ReportModelBuilder : FrmBaseModelBuilder, IModelBuilder<IEnumerable<Frm08ReportModel>>
    {
        private readonly int _pausedCompStatus = 6;
        private readonly int _excludedAimType = 3;
        private readonly HashSet<int> _excludedFundModel = new HashSet<int> { 25, 99 };
        private readonly HashSet<int> _excludedCategories = new HashSet<int> { 23, 24, 27, 28, 29, 34, 35, 36 };

        public IEnumerable<Frm08ReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var models = new List<Frm08ReportModel>();
            var returnPeriod = $"R{reportServiceContext.ReturnPeriod:D2}";

            var message = reportServiceDependentData.Get<IMessage>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var organisationNameDictionary = referenceData.Organisations.ToDictionary(x => x.UKPRN, x => x.Name);

            var orgName = organisationNameDictionary.GetValueOrDefault(reportServiceContext.Ukprn);

            var pausedDeliveries = message.Learners
                                        ?.SelectMany(l => l.LearningDeliveries.Where(ld => 
                                            ld.CompStatus == _pausedCompStatus 
                                            && !ExcludedDelivery(ld, referenceData.LARSLearningDeliveries)
                                            && ld.AimType != _excludedAimType
                                            && !_excludedFundModel.Contains(ld.FundModel))
                                            .Select(ld => new {Learner = l, LearningDelivery = ld}));

            var currentReturnEndDate = referenceData.MetaDatas.CollectionDates.ReturnPeriods.FirstOrDefault(d =>
                                                                                    reportServiceContext.SubmissionDateTimeUtc >= d.Start 
                                                                                    && reportServiceContext.SubmissionDateTimeUtc <= d.End).End;

            if (pausedDeliveries == null)
            {
                return Enumerable.Empty<Frm08ReportModel>();
            }

            foreach (var delivery in pausedDeliveries)
            {
                var restartDelivery = GetRestartDelivery(delivery.LearningDelivery, delivery.Learner);

                if (restartDelivery != null)
                {
                    continue;
                }

                if (DaysBetween(delivery.LearningDelivery.LearnActEndDateNullable.Value, currentReturnEndDate) >= 365)
                {
                    var advancedLoansIndicator = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, ADLLearnDelFamType);
                    var devolvedIndicator = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, SOFLearnDelFamType);
                    var resIndicator = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, RESLearnDelFamType);

                    var partnerOrgName = organisationNameDictionary.GetValueOrDefault(delivery.LearningDelivery.PartnerUKPRNNullable.GetValueOrDefault());

                    models.Add(new Frm08ReportModel
                    {
                        UKPRN = reportServiceContext.Ukprn,
                        Return = returnPeriod,
                        OrgName = orgName,
                        FworkCode = delivery.LearningDelivery.FworkCodeNullable,
                        LearnAimRef = delivery.LearningDelivery.LearnAimRef,
                        LearnRefNumber = delivery.Learner.LearnRefNumber,
                        LearnStartDate = delivery.LearningDelivery.LearnStartDate,
                        ProgType = delivery.LearningDelivery.ProgTypeNullable,
                        StdCode = delivery.LearningDelivery.StdCodeNullable,
                        ULN = delivery.Learner.ULN,
                        AdvancedLoansIndicator = advancedLoansIndicator,
                        AimSeqNumber = delivery.LearningDelivery.AimSeqNumber,
                        CompStatus = delivery.LearningDelivery.CompStatus,
                        LearnActEndDate = delivery.LearningDelivery.LearnActEndDateNullable,
                        LearnPlanEndDate = delivery.LearningDelivery.LearnPlanEndDate,
                        OtherFundAdj = delivery.LearningDelivery.OtherFundAdjNullable,
                        Outcome = delivery.LearningDelivery.OutcomeNullable,
                        PMUKPRN = delivery.Learner.PMUKPRNNullable,
                        PartnerUKPRN = delivery.LearningDelivery.PartnerUKPRNNullable,
                        PartnerOrgName = partnerOrgName,
                        PriorLearnFundAdj = delivery.LearningDelivery.PriorLearnFundAdjNullable,
                        PrevLearnRefNumber = delivery.Learner.PrevLearnRefNumber,
                        PrevUKPRN = delivery.Learner.PrevUKPRNNullable,
                        PwayCode = delivery.LearningDelivery.PwayCodeNullable,
                        ResIndicator = resIndicator,
                        SWSupAimId = delivery.LearningDelivery.SWSupAimId,
                        ProvSpecLearnDelMon = string.Join(";", delivery.LearningDelivery.ProviderSpecDeliveryMonitorings.Select(x => x.ProvSpecDelMon)),
                        ProvSpecDelMon = string.Join(";", delivery.Learner.ProviderSpecLearnerMonitorings.Select(x => x.ProvSpecLearnMon)),
                        FundingStream = CalculateFundingStream(delivery.LearningDelivery.FundModel, delivery.LearningDelivery.ProgTypeNullable, advancedLoansIndicator, devolvedIndicator)
                    });
                }
            }

            return models;
        }

        private ILearningDelivery GetRestartDelivery(ILearningDelivery breakLearningDelivery, ILearner learner)
        {
            return learner.LearningDeliveries.FirstOrDefault(ld => ld.LearnAimRef.CaseInsensitiveEquals(breakLearningDelivery.LearnAimRef)
                                                                   && ld.ProgTypeNullable == breakLearningDelivery.ProgTypeNullable
                                                                   && ld.StdCodeNullable == breakLearningDelivery.StdCodeNullable
                                                                   && ld.FworkCodeNullable == breakLearningDelivery.FworkCodeNullable
                                                                   && HasRestartFAM(ld.LearningDeliveryFAMs)
                                                                   && WithMatchingStartDates(breakLearningDelivery, ld));
        }

        private bool HasRestartFAM(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return learningDeliveryFams.Any(f => f.LearnDelFAMType.Equals(RESLearnDelFamType, StringComparison.OrdinalIgnoreCase));
        }

        private bool WithMatchingStartDates(ILearningDelivery breakLearningDelivery, ILearningDelivery learningDelivery)
        {
            if (learningDelivery.OrigLearnStartDateNullable == null)
            {
                return false;
            }

            return (learningDelivery.OrigLearnStartDateNullable.Value == breakLearningDelivery.LearnStartDate 
                || learningDelivery.OrigLearnStartDateNullable.Value == breakLearningDelivery.OrigLearnStartDateNullable)
                && learningDelivery.LearnStartDate > breakLearningDelivery.LearnActEndDateNullable;
        }

        private bool ExcludedDelivery(ILearningDelivery learner, IReadOnlyCollection<LARSLearningDelivery> larsLearningDeliveries)
        {
            return larsLearningDeliveries
                .Any(x => x.LearnAimRef.CaseInsensitiveEquals(learner.LearnAimRef)
                          && x.LARSLearningDeliveryCategories.Any(ldc => _excludedCategories.Contains(ldc.CategoryRef)));
        }

        public double DaysBetween(DateTime start, DateTime end)
        {
            return (end - start).TotalDays;
        }
    }
}
