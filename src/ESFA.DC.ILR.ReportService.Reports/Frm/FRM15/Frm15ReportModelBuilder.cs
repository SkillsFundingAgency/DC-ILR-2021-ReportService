using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM15
{
    public class Frm15ReportModelBuilder : FrmBaseModelBuilder, IModelBuilder<IEnumerable<Frm15ReportModel>>
    {
        private readonly int _includedAimType = 1;
        private readonly int _includedFundModel = 36;
        private readonly int _includedProgType = 25;

        private readonly string AFinTypeTNP = "TNP";
        private readonly string AFinTypePMR = "PMR";
        private readonly int AFinCode = 2;

        public IEnumerable<Frm15ReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var models = new List<Frm15ReportModel>();
            var returnPeriod = reportServiceContext.ReturnPeriodName;

            var message = reportServiceDependentData.Get<IMessage>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var organisationNameDictionary = referenceData.Organisations.ToDictionary(x => x.UKPRN, x => x.Name);

            var learnAimDictionary = referenceData.LARSLearningDeliveries.ToDictionary(x => x.LearnAimRef, x => x.LearnAimRefTitle, StringComparer.OrdinalIgnoreCase);

            var orgName = organisationNameDictionary.GetValueOrDefault(reportServiceContext.Ukprn);

            var deliveries = message.Learners
                                        ?.SelectMany(l => l.LearningDeliveries.Where(ld =>
                                            ld.FundModel == _includedFundModel
                                            && ld.ProgTypeNullable == _includedProgType
                                            && ld.AimType == _includedAimType
                                            && ld.EPAOrgID == null).Select(ld => new { Learner = l, LearningDelivery = ld }));

            var currentReturnEndDate = referenceData.MetaDatas.CollectionDates.ReturnPeriods.FirstOrDefault(d => reportServiceContext.SubmissionDateTimeUtc >= d.Start && reportServiceContext.SubmissionDateTimeUtc <= d.End).End;

            if (deliveries == null)
            {
                return Enumerable.Empty<Frm15ReportModel>();
            }

            foreach (var delivery in deliveries)
            {
                if ((delivery.LearningDelivery.LearnPlanEndDate > currentReturnEndDate && DaysBetween(currentReturnEndDate, delivery.LearningDelivery.LearnPlanEndDate) <= 90) || currentReturnEndDate > delivery.LearningDelivery.LearnPlanEndDate)
                {
                    var aFinAmount = delivery.LearningDelivery.AppFinRecords
                        ?.OrderByDescending(afr => afr.AFinDate).FirstOrDefault(afr => afr.AFinType == AFinTypeTNP && afr.AFinCode == AFinCode)?.AFinAmount;

                    var paymentsReceived = delivery.LearningDelivery.AppFinRecords
                        ?.Where(afr => afr.AFinType == AFinTypePMR && afr.AFinCode == AFinCode)
                        .Sum(afr => afr.AFinAmount);

                    var advancedLoansIndicator = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, ADLLearnDelFamType);
                    var devolvedIndicator = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, SOFLearnDelFamType);
                    var resIndicator = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, RESLearnDelFamType);

                    var partnerOrgName = organisationNameDictionary.GetValueOrDefault(delivery.LearningDelivery.PartnerUKPRNNullable.GetValueOrDefault());
                    var learnAimTitle = learnAimDictionary.GetValueOrDefault(delivery.LearningDelivery.LearnAimRef);

                    models.Add(new Frm15ReportModel
                    {
                        UKPRN = reportServiceContext.Ukprn,
                        Return = returnPeriod,
                        OrgName = orgName,
                        FworkCode = delivery.LearningDelivery.FworkCodeNullable,
                        LearnAimRef = delivery.LearningDelivery.LearnAimRef,
                        LearnAimTitle = learnAimTitle,
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
                        OtjActHours = delivery.LearningDelivery.OtjActHoursNullable,
                        Outcome = delivery.LearningDelivery.OutcomeNullable,
                        PMUKPRN = delivery.Learner.PMUKPRNNullable,
                        PartnerUKPRN = delivery.LearningDelivery.PartnerUKPRNNullable,
                        PartnerOrgName = partnerOrgName,
                        PriorLearnFundAdj = delivery.LearningDelivery.PriorLearnFundAdjNullable,
                        PrevLearnRefNumber = delivery.Learner.PrevLearnRefNumber,
                        PrevUKPRN = delivery.Learner.PrevUKPRNNullable,
                        PwayCode = delivery.LearningDelivery.PwayCodeNullable,
                        ResIndicator = resIndicator,
                        EPAOrgId = delivery.LearningDelivery.EPAOrgID,
                        SWSupAimId = delivery.LearningDelivery.SWSupAimId,
                        ProvSpecLearnDelMon = ProviderSpecDeliveryMonitorings(delivery.LearningDelivery.ProviderSpecDeliveryMonitorings),
                        ProvSpecDelMon = ProviderSpecLearningMonitorings(delivery.Learner.ProviderSpecLearnerMonitorings),
                        FundingStream = CalculateFundingStream(delivery.LearningDelivery.FundModel, delivery.LearningDelivery.ProgTypeNullable, advancedLoansIndicator, devolvedIndicator),
                        TotalNegotiatedAssessmentPrice = aFinAmount,
                        AssessmentPaymentReceived = paymentsReceived
                    });
                }
            }

            return models;
        }

        private double DaysBetween(DateTime start, DateTime end)
        {
            return (end - start).TotalDays;
        }
    }
}
