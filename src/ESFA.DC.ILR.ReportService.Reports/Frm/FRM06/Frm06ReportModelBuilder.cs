using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.FRM;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM06
{
    public class Frm06ReportModelBuilder : IModelBuilder<IEnumerable<Frm06ReportModel>>
    {
        private const string ADLLearnDelFamType = "ADL";
        private const string SOFLearnDelFamType = "SOF";
        private const string RESLearnDelFamType = "RES";

        private readonly HashSet<int> _apprenticeshipHashSet = new HashSet<int> { 2, 3, 20, 21, 22, 23, 25 };
        private readonly HashSet<string> _devolvedCodes = new HashSet<string> { "110", "111", "112", "113", "114", "115", "116" };

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
            var currentLearnersHashSet = BuildCurrentYearLearnerHashSet(message);

            var returnPeriod = $"R{reportServiceContext.ReturnPeriod:D2}";

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

                if (!currentLearnersHashSet.Contains(key, _frmEqualityComparer))
                {
                    var advancedLoansIndicator = RetrieveFamCodeForType(learner.LearningDeliveryFAMs, ADLLearnDelFamType);
                    var devolvedIndicator = RetrieveFamCodeForType(learner.LearningDeliveryFAMs, SOFLearnDelFamType);
                    var resIndicator = RetrieveFamCodeForType(learner.LearningDeliveryFAMs, RESLearnDelFamType);

                    models.Add(new Frm06ReportModel
                    {
                        UKPRN = learner.UKPRN,
                        Return = returnPeriod,
                        OrgName = learner.OrgName,
                        FworkCode = learner.FworkCodeNullable,
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
                        FundingStream = CalculateFundingStream(learner, advancedLoansIndicator, devolvedIndicator)
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
                })) ?? Enumerable.Empty<FrmLearnerKey>());
        }

        private string RetrieveFamCodeForType(IEnumerable<LearningDeliveryFAM> deliveryFams, string learnDelFamType)
        {
            return deliveryFams.FirstOrDefault(f => f.LearnDelFAMType == learnDelFamType)?.LearnDelFAMCode ?? string.Empty;
        }

        private string CalculateFundingStream(FrmLearner learner, string advancedLearnerLoansIndicator, string devolvedIndicator)
        {
            if (_devolvedCodes.Contains(devolvedIndicator))
            {
                return FundingStreamConstants.Devolved;
            }

            if (learner.FundModel == 35 && !learner.ProgTypeNullable.HasValue)
            {
                return FundingStreamConstants.AEB;
            }
        
            if ((learner.FundModel == 35 && learner.ProgTypeNullable.HasValue && _apprenticeshipHashSet.Contains(learner.ProgTypeNullable.Value)) || learner.FundModel == 81)
            {
                return FundingStreamConstants.Apprenticeships;
            }
        
            if (learner.FundModel == 25 && learner.ProgTypeNullable.HasValue && learner.ProgTypeNullable.Value == 24)
            {
                return FundingStreamConstants.Traineeships1618;
            }
        
            if (learner.FundModel == 35 && learner.ProgTypeNullable.HasValue && learner.ProgTypeNullable.Value == 24)
            {
                return FundingStreamConstants.Traineeships1924;
            }
        
            if (learner.FundModel == 25 && learner.ProgTypeNullable.HasValue && learner.ProgTypeNullable.Value != 24)
            {
                return FundingStreamConstants.SixteenToNineteen;
            }
        
            if (learner.FundModel == 36)
            {
                return FundingStreamConstants.ApprenticeshipsFromMay2017;
            }
        
            if (learner.FundModel == 99 && advancedLearnerLoansIndicator == "1")
            {
                return FundingStreamConstants.AdvancedLearnerLoans;
            }
        
            if (learner.FundModel == 99 && advancedLearnerLoansIndicator != "1")
            {
                return FundingStreamConstants.NonFunded;
            }

            return FundingStreamConstants.Other;
        }
    }
}
