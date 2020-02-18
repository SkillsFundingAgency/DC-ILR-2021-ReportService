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
                    var advancedLoansIndicator = learner.LearningDeliveryFAMs.FirstOrDefault(fam => fam.LearnDelFAMType == ADLLearnDelFamType)?.LearnDelFAMCode ?? string.Empty;

                    var devolvedIndicator = learner.LearningDeliveryFAMs.FirstOrDefault(fam => fam.LearnDelFAMType == SOFLearnDelFamType)?.LearnDelFAMCode ?? string.Empty;

                    models.Add(new Frm06ReportModel
                    {
                        UKPRN = learner.UKPRN,
                        Return = $"R{reportServiceContext.ReturnPeriod:D2}",
                        OrgName = learner.OrgName,
                        FworkCode = learner.FworkCodeNullable?.ToString() ?? string.Empty,
                        LearnAimRef = learner.LearnAimRef,
                        LearnRefNumber = learner.LearnRefNumber,
                        LearnStartDate = learner.LearnStartDate,
                        ProgType = learner.ProgTypeNullable?.ToString() ?? string.Empty,
                        StdCode = learner.StdCodeNullable?.ToString() ?? string.Empty,
                        ULN = learner.ULN,
                        AdvancedLoansIndicator = advancedLoansIndicator,
                        AimSeqNumber = learner.AimSeqNumber,
                        CompStatus = learner.CompStatus,
                        LearnActEndDate = learner.LearnActEndDate,
                        LearnPlanEndDate = learner.LearnPlanEndDate,
                        OtherFundAdj = learner.OtherFundAdj?.ToString() ?? string.Empty,
                        Outcome = learner.Outcome?.ToString() ?? string.Empty,
                        PMUKPRN = learner.PMUKPRN?.ToString() ?? string.Empty,
                        PartnerUKPRN = learner.PartnerUKPRN?.ToString() ?? string.Empty,
                        PartnerOrgName = learner.PartnerOrgName,
                        PriorLearnFundAdj = learner.PriorLearnFundAdj?.ToString() ?? string.Empty,
                        PrevLearnRefNumber = learner.PrevLearnRefNumber,
                        PrevUKPRN = learner.PrevUKPRN?.ToString() ?? string.Empty,
                        PwayCode = learner.PwayCodeNullable?.ToString() ?? string.Empty,
                        ResIndicator = learner.LearningDeliveryFAMs.FirstOrDefault(fam => fam.LearnDelFAMType == RESLearnDelFamType)?.LearnDelFAMCode ?? string.Empty,
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
