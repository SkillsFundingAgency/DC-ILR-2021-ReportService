using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Models.Ilr
{
    public class MessageLearner : ILearner
    {
        public string LearnRefNumber { get; set; }

        public IReadOnlyCollection<ILearningDelivery> LearningDeliveries => LearningDeliveriesData;

        public List<MessageLearnerLearningDelivery> LearningDeliveriesData { get; set; }

        public int? AccomNullable => throw new NotImplementedException();

        public string AddLine1 => throw new NotImplementedException();

        public string AddLine2 => throw new NotImplementedException();

        public string AddLine3 => throw new NotImplementedException();

        public string AddLine4 => throw new NotImplementedException();

        public int? ALSCostNullable => throw new NotImplementedException();

        public string CampId => throw new NotImplementedException();

        public DateTime? DateOfBirthNullable => throw new NotImplementedException();

        public string Email => throw new NotImplementedException();

        public int Ethnicity => throw new NotImplementedException();

        public string EngGrade => throw new NotImplementedException();

        public string FamilyName => throw new NotImplementedException();

        public string GivenNames => throw new NotImplementedException();

        public int LLDDHealthProb => throw new NotImplementedException();

        public string MathGrade => throw new NotImplementedException();

        public string NINumber => throw new NotImplementedException();

        public int? PlanEEPHoursNullable => throw new NotImplementedException();

        public int? PlanLearnHoursNullable => throw new NotImplementedException();

        public int? PMUKPRNNullable => throw new NotImplementedException();

        public string Postcode => throw new NotImplementedException();

        public string PostcodePrior => throw new NotImplementedException();

        public string PrevLearnRefNumber => throw new NotImplementedException();

        public int? PrevUKPRNNullable => throw new NotImplementedException();

        public int? PriorAttainNullable => throw new NotImplementedException();

        public long ULN => throw new NotImplementedException();

        public string Sex => throw new NotImplementedException();

        public string TelNo => throw new NotImplementedException();

        public ILearnerHE LearnerHEEntity => throw new NotImplementedException();

        public IReadOnlyCollection<IContactPreference> ContactPreferences => throw new NotImplementedException();

        public IReadOnlyCollection<ILearnerFAM> LearnerFAMs => throw new NotImplementedException();

        public IReadOnlyCollection<ILLDDAndHealthProblem> LLDDAndHealthProblems => throw new NotImplementedException();

        public IReadOnlyCollection<IProviderSpecLearnerMonitoring> ProviderSpecLearnerMonitorings => throw new NotImplementedException();

        public IReadOnlyCollection<ILearnerEmploymentStatus> LearnerEmploymentStatuses => throw new NotImplementedException();
    }
}
