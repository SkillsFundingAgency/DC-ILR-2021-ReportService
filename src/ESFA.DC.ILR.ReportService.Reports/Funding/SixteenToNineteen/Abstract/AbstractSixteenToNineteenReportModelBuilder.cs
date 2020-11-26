using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract
{
    public abstract class AbstractSixteenToNineteenReportModelBuilder : AbstractReportModelBuilder
    {
        private readonly IDictionary<string, int?> _fundModelDictionary =
              new Dictionary<string, int?>(StringComparer.OrdinalIgnoreCase)
              {
                  [FundLineConstants.DirectFundedStudents1416] = 0,
                  [FundLineConstants.StudentsExcludingHighNeeds1619] = 1,
                  [FundLineConstants.HighNeedsStudents1619] = 2,
                  [FundLineConstants.StudentsWithEHCP1924] = 3,
                  [FundLineConstants.ContinuingStudents19Plus] = 4,
              };

        public virtual bool Filter(ILearner learner, FM25Learner fm25Learner)
        {
            return learner != null && fm25Learner != null
                   && FilterStartFund(fm25Learner.StartFund)
                   && FilterFundLine(fm25Learner.FundLine)
                   && learner?.LearningDeliveries?.Any(ld => ld?.LearningDeliveryFAMs?.Any(FilterSOF) == true) == true;
        }

        public bool FilterStartFund(bool? startFund) => startFund == true;

        public bool FilterFundLine(string fundLine) => fundLine != null && _fundModelDictionary.ContainsKey(fundLine);

        public bool StudyProgrammePredicate(FM25Learner fM25Learner) => fM25Learner.TLevelStudent == false;
        public bool TLevelPredicate(FM25Learner fM25Learner) => fM25Learner.TLevelStudent == true;

        public bool FilterSOF(ILearningDeliveryFAM learningDeliveryFam)
            => learningDeliveryFam?.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.SOF) == true
                           && learningDeliveryFam?.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMCodeConstants.SOF_ESFA_1619) == true;

        protected IDictionary<string, FM25Learner> BuildFm25LearnerDictionary(FM25Global fm25Global)
            => fm25Global?
                  .Learners?
                  .ToDictionary(l => l.LearnRefNumber, l => l, StringComparer.OrdinalIgnoreCase) ??
              new Dictionary<string, FM25Learner>();

        public IEnumerable<T> Order<T>(IEnumerable<T> models)
            where T : AbstractSixteenToNineteenModel
        {
            return models
                .OrderBy(m => _fundModelDictionary.GetValueOrDefault(m.FM25Learner.FundLine) ?? int.MaxValue)
                .ThenBy(m => m.Learner.LearnRefNumber);
        }
    }
}
