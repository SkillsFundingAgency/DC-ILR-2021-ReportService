﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.FRM;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM06
{
    public class Frm06ReportModelBuilder : FrmBaseModelBuilder, IModelBuilder<IEnumerable<Frm06ReportModel>>
    {
        private readonly IEqualityComparer<FrmLearnerKey> _frmEqualityComparer;

        public Frm06ReportModelBuilder(IEqualityComparer<FrmLearnerKey> frmEqualityComparer)
        {
            _frmEqualityComparer = frmEqualityComparer;
        }

        public IEnumerable<Frm06ReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var models = new List<Frm06ReportModel>();

            var frmLearners = reportServiceDependentData.Get<FrmReferenceData>();
            var message = reportServiceDependentData.Get<IMessage>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var currentLearnersHashSet = BuildCurrentYearLearnerHashSet(message);

            var learnAimDictionary = referenceData.LARSLearningDeliveries.ToDictionary(x => x.LearnAimRef, x => x.LearnAimRefTitle, StringComparer.OrdinalIgnoreCase);

            var returnPeriod = reportServiceContext.ReturnPeriodName;

            foreach (var learner in frmLearners?.Frm06Learners ?? Enumerable.Empty<FrmLearner>())
            {
                var key = new FrmLearnerKey
                {
                    FworkCodeNullable = learner.FworkCodeNullable,
                    LearnAimRef = learner.LearnAimRef,
                    LearnRefNumber = learner.LearnRefNumber,
                    LearnStartDate = learner.LearnStartDate,
                    ProgTypeNullable = learner.ProgTypeNullable,
                    StdCodeNullable = learner.StdCodeNullable
                };

                if (!currentLearnersHashSet.Contains(key))
                {
                    var advancedLoansIndicator = RetrieveFamCodeForType(learner.LearningDeliveryFAMs, ADLLearnDelFamType);
                    var devolvedIndicator = RetrieveFamCodeForType(learner.LearningDeliveryFAMs, SOFLearnDelFamType);
                    var resIndicator = RetrieveFamCodeForType(learner.LearningDeliveryFAMs, RESLearnDelFamType);
                    var learnAimTitle = learnAimDictionary.GetValueOrDefault(learner.LearnAimRef);

                    models.Add(new Frm06ReportModel
                    {
                        UKPRN = learner.UKPRN,
                        Return = returnPeriod,
                        OrgName = learner.OrgName,
                        FworkCode = learner.FworkCodeNullable,
                        LearnAimTitle = learnAimTitle,
                        LearnAimRef = learner.LearnAimRef,
                        LearnRefNumber = learner.LearnRefNumber,
                        LearnStartDate = learner.LearnStartDate,
                        ProgType = learner.ProgTypeNullable,
                        StdCode = learner.StdCodeNullable,
                        ULN = learner.ULN,
                        AdvancedLoansIndicator = advancedLoansIndicator,
                        AimSeqNumber = learner.AimSeqNumber,
                        CompStatus = learner.CompStatus,
                        LearnActEndDate = learner.LearnActEndDate,
                        LearnPlanEndDate = learner.LearnPlanEndDate,
                        OtherFundAdj = learner.OtherFundAdj,
                        Outcome = learner.Outcome,
                        PMUKPRN = learner.PMUKPRN,
                        PartnerUKPRN = learner.PartnerUKPRN,
                        PartnerOrgName = learner.PartnerOrgName,
                        PriorLearnFundAdj = learner.PriorLearnFundAdj,
                        PrevLearnRefNumber = learner.PrevLearnRefNumber,
                        PrevUKPRN = learner.PrevUKPRN,
                        PwayCode = learner.PwayCodeNullable,
                        ResIndicator = resIndicator,
                        SWSupAimId = learner.SWSupAimId,
                        ProvSpecLearnDelMon = string.Join(";", learner.ProvSpecDeliveryMonitorings.Select(x => x.ProvSpecDelMon)),
                        ProvSpecDelMon = string.Join(";", learner.ProviderSpecLearnerMonitorings.Select(x => x.ProvSpecLearnMon)),
                        FundingStream = CalculateFundingStream(learner.FundModel, learner.ProgTypeNullable, advancedLoansIndicator, devolvedIndicator)
                    });
                }
            }

            return models;
        }

        private HashSet<FrmLearnerKey> BuildCurrentYearLearnerHashSet(IMessage message)
        {
            return new HashSet<FrmLearnerKey>(message.Learners?
                .SelectMany(l => l?.LearningDeliveries
                .Select(ld => new FrmLearnerKey
                {
                    FworkCodeNullable = ld.FworkCodeNullable,
                    LearnAimRef = ld.LearnAimRef,
                    LearnRefNumber = l.LearnRefNumber,
                    LearnStartDate = ld.LearnStartDate,
                    ProgTypeNullable = ld.ProgTypeNullable,
                    StdCodeNullable = ld.StdCodeNullable
                })) ?? Enumerable.Empty<FrmLearnerKey>(), _frmEqualityComparer);
        }
    }
}