using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM15
{
    public class Frm15ReportModelBuilder : FrmBaseModelBuilder, IModelBuilder<IEnumerable<Frm15ReportModel>>
    {
        private readonly int _includedCompStatus = 1;
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

            var learnAimDictionary = referenceData.LARSLearningDeliveries.ToDictionary(x => x.LearnAimRef, x => x, StringComparer.OrdinalIgnoreCase);

            var orgName = organisationNameDictionary.GetValueOrDefault(reportServiceContext.Ukprn);

            var deliveries = message.Learners
                                        ?.SelectMany(l => l.LearningDeliveries.Where(ld =>
                                            ld.FundModel == _includedFundModel
                                            && ld.ProgTypeNullable == _includedProgType
                                            && ld.AimType == _includedAimType
                                            && ld.CompStatus == _includedCompStatus
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
                    var sofCode = RetrieveFamCodeForType(delivery.LearningDelivery.LearningDeliveryFAMs, SOFLearnDelFamType);

                    var pmOrgName = organisationNameDictionary.GetValueOrDefault(
                            delivery.Learner.PMUKPRNNullable.GetValueOrDefault());
                    var prevOrgName = organisationNameDictionary.GetValueOrDefault(delivery.Learner.PrevUKPRNNullable
                            .GetValueOrDefault());
                    var partnerOrgName = organisationNameDictionary.GetValueOrDefault(delivery.LearningDelivery.PartnerUKPRNNullable.GetValueOrDefault());
                    var learnAim = learnAimDictionary.GetValueOrDefault(delivery.LearningDelivery.LearnAimRef);

                    models.Add(new Frm15ReportModel
                    {
                        Return = returnPeriod,
                        UKPRN = reportServiceContext.Ukprn,
                        OrgName = orgName,
                        PartnerUKPRN = delivery.LearningDelivery.PartnerUKPRNNullable,
                        PartnerOrgName = partnerOrgName,
                        PrevUKPRN = delivery.Learner.PrevUKPRNNullable,
                        PrevOrgName = prevOrgName,
                        PMUKPRN = delivery.Learner.PMUKPRNNullable,
                        PMOrgName = pmOrgName,
                        ULN = delivery.Learner.ULN,
                        LearnRefNumber = delivery.Learner.LearnRefNumber,
                        SWSupAimId = delivery.LearningDelivery.SWSupAimId,
                        LearnAimRef = delivery.LearningDelivery.LearnAimRef,
                        LearnAimTitle = learnAim.LearnAimRefTitle,
                        AimSeqNumber = delivery.LearningDelivery.AimSeqNumber,
                        AimTypeCode = delivery.LearningDelivery.AimType,
                        LearnAimType = learnAim.LearnAimRefTypeDesc,
                        StdCode = delivery.LearningDelivery.StdCodeNullable,
                        FworkCode = delivery.LearningDelivery.FworkCodeNullable,
                        PwayCode = delivery.LearningDelivery.PwayCodeNullable,
                        ProgType = delivery.LearningDelivery.ProgTypeNullable,
                        LearnStartDate = delivery.LearningDelivery.LearnStartDate,
                        OrigLearnStartDate = delivery.LearningDelivery.OrigLearnStartDateNullable,
                        LearnPlanEndDate = delivery.LearningDelivery.LearnPlanEndDate,
                        LearnActEndDate = delivery.LearningDelivery.LearnActEndDateNullable,
                        CompStatus = delivery.LearningDelivery.CompStatus,
                        Outcome = delivery.LearningDelivery.OutcomeNullable,
                        FundModel = delivery.LearningDelivery.FundModel,
                        SOFCode = sofCode,
                        AdvancedLoansIndicator = advancedLoansIndicator,
                        ResIndicator = resIndicator,
                        ProvSpecLearnDelMon = ProviderSpecDeliveryMonitorings(delivery.LearningDelivery.ProviderSpecDeliveryMonitorings),
                        ProvSpecDelMon = ProviderSpecLearningMonitorings(delivery.Learner.ProviderSpecLearnerMonitorings),
                        PriorLearnFundAdj = delivery.LearningDelivery.PriorLearnFundAdjNullable,
                        OtherFundAdj = delivery.LearningDelivery.OtherFundAdjNullable,
                        EPAOrgId = delivery.LearningDelivery.EPAOrgID,
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
