using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentDetail
{
    public class HighNeedsStudentDetailReportModelBuilder : AbstractSixteenToNineteenReportModelBuilder, IModelBuilder<IEnumerable<HighNeedsStudentDetailReportModel>>
    {
        private readonly IDictionary<string, int?> _fundlineSortOrderDictionary =
              new Dictionary<string, int?>(System.StringComparer.OrdinalIgnoreCase)
              {
                  [FundLineConstants.DirectFundedStudents1416] = 0,
                  [FundLineConstants.StudentsIncludingHighNeeds1619] = 1,
                  [FundLineConstants.HighNeedsStudents1619] = 2,
                  [FundLineConstants.StudentsWithEHCP1924] = 3,
                  [FundLineConstants.ContinuingStudents19Plus] = 4,
              };

        private readonly IDictionary<string, string> _derivedFundLineDictionary = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
        {
            { FundLineConstants.DirectFundedStudents1416, FundLineConstants.DirectFundedStudents1416 },
            { FundLineConstants.StudentsExcludingHighNeeds1619, FundLineConstants.StudentsIncludingHighNeeds1619 },
            { FundLineConstants.HighNeedsStudents1619, FundLineConstants.StudentsIncludingHighNeeds1619 },
            { FundLineConstants.StudentsWithEHCP1924, FundLineConstants.StudentsWithEHCP1924 },
            { FundLineConstants.ContinuingStudents19Plus, FundLineConstants.ContinuingStudents19Plus }
        };

        public IEnumerable<HighNeedsStudentDetailReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm25 = reportServiceDependentData.Get<FM25Global>();

            var fm25LearnerDictionary = BuildFm25LearnerDictionary(fm25);

            var models = new List<HighNeedsStudentDetailReportModel>();

            foreach (var learner in message?.Learners ?? Enumerable.Empty<ILearner>())
            {
                var fm25Learner = fm25LearnerDictionary.GetValueOrDefault(learner.LearnRefNumber);

                if (Filter(learner, fm25Learner))
                {
                    var ehc = FamCodeForType(learner.LearnerFAMs, LearnerFAMTypeConstants.EHC);
                    var hns = FamCodeForType(learner.LearnerFAMs, LearnerFAMTypeConstants.HNS);

                    models.Add(new HighNeedsStudentDetailReportModel()
                    {
                        Learner = learner,
                        FM25Learner = fm25Learner,
                        DerivedFundline = _derivedFundLineDictionary.GetValueOrDefault(fm25Learner.FundLine),
                        StudentsWithAnEhcp = StudentWithAnEhcp(ehc),
                        StudentsWithoutAnEhcp = StudentWithoutAnEhcp(ehc),
                        HighNeedsStudentsWithoutAnEhcp = HighNeedsStudentWithoutAnEhcp(hns, ehc),
                        StudentsWithAnEhcpAndHns = StudentWithAnEhcpAndHns(hns, ehc),
                        StudentWithAnEhcpAndNotHns = StudentWithAnEhcpAndNotHns(hns, ehc),
                    });
                }
            }

            return Order(models);
        }

        public bool StudentWithAnEhcp(int? ehcp) => ehcp == 1;

        public bool StudentWithoutAnEhcp(int? ehcp) => !ehcp.HasValue;

        public bool HighNeedsStudentWithoutAnEhcp(int? hns, int? ehc) => hns == 1 && !ehc.HasValue;

        public bool StudentWithAnEhcpAndHns(int? hns, int? ehc) => hns == 1 && ehc == 1;

        public bool StudentWithAnEhcpAndNotHns(int? hns, int? ehc) => !hns.HasValue && ehc == 1;

        public IEnumerable<HighNeedsStudentDetailReportModel> Order(IEnumerable<HighNeedsStudentDetailReportModel> models)
        {
            return models
                .OrderBy(m => _fundlineSortOrderDictionary.GetValueOrDefault(m.DerivedFundline) ?? int.MaxValue)
                .ThenBy(m => m.Learner.LearnRefNumber);
        }

        private int? FamCodeForType(IEnumerable<ILearnerFAM> learnerFams, string type) => learnerFams?.FirstOrDefault(fam => fam.LearnFAMType.CaseInsensitiveEquals(type))?.LearnFAMCode;
    }
}
