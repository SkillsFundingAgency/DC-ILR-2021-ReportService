using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Models.Ilr
{
    public class MessageLearnerLearningDelivery : ILearningDelivery
    {
        public int AimSeqNumber { get; set; }

        public IReadOnlyCollection<ILearningDeliveryFAM> LearningDeliveryFAMs => LearningDeliveryFAMsData;

        public List<MessageLearnerLearningDeliveryLearningDeliveryFAM> LearningDeliveryFAMsData { get; set; }

        public DateTime? AchDateNullable => throw new NotImplementedException();

        public int? AddHoursNullable => throw new NotImplementedException();

        public int AimType => throw new NotImplementedException();

        public int CompStatus => throw new NotImplementedException();

        public string ConRefNumber => throw new NotImplementedException();

        public string DelLocPostCode => throw new NotImplementedException();

        public int? EmpOutcomeNullable => throw new NotImplementedException();

        public string EPAOrgID => throw new NotImplementedException();

        public int FundModel => throw new NotImplementedException();

        public int? FworkCodeNullable => throw new NotImplementedException();

        public string LearnAimRef => throw new NotImplementedException();

        public DateTime? LearnActEndDateNullable => throw new NotImplementedException();

        public DateTime LearnPlanEndDate => throw new NotImplementedException();

        public DateTime LearnStartDate => throw new NotImplementedException();

        public string LSDPostcode => throw new NotImplementedException();

        public DateTime? OrigLearnStartDateNullable => throw new NotImplementedException();

        public int? OtherFundAdjNullable => throw new NotImplementedException();

        public int? OtjActHoursNullable => throw new NotImplementedException();

        public int? OutcomeNullable => throw new NotImplementedException();

        public string OutGrade => throw new NotImplementedException();

        public int? PartnerUKPRNNullable => throw new NotImplementedException();

        public int? PHoursNullable => throw new NotImplementedException();

        public int? PriorLearnFundAdjNullable => throw new NotImplementedException();

        public int? ProgTypeNullable => throw new NotImplementedException();

        public int? PwayCodeNullable => throw new NotImplementedException();

        public int? StdCodeNullable => throw new NotImplementedException();

        public string SWSupAimId => throw new NotImplementedException();

        public int? WithdrawReasonNullable => throw new NotImplementedException();

        public ILearningDeliveryHE LearningDeliveryHEEntity => throw new NotImplementedException();

        public IReadOnlyCollection<IAppFinRecord> AppFinRecords => throw new NotImplementedException();

        public IReadOnlyCollection<ILearningDeliveryWorkPlacement> LearningDeliveryWorkPlacements => throw new NotImplementedException();

        public IReadOnlyCollection<IProviderSpecDeliveryMonitoring> ProviderSpecDeliveryMonitorings => throw new NotImplementedException();
    }
}
