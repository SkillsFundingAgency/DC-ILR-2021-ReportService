using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.SummaryOfFundingByStudent
{
    public class SummaryOfFundingByStudentModelBuilder : AbstractSixteenToNineteenReportModelBuilder, IModelBuilder<IEnumerable<SummaryOfFundingByStudentReportModel>>
    {
        public IEnumerable<SummaryOfFundingByStudentReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm25 = reportServiceDependentData.Get<FM25Global>();

            var fm25LearnerDictionary = BuildFm25LearnerDictionary(fm25);

            var models = new List<SummaryOfFundingByStudentReportModel>();

            foreach (var learner in message?.Learners ?? Enumerable.Empty<ILearner>())
            {
                var fm25Learner = fm25LearnerDictionary.GetValueOrDefault(learner.LearnRefNumber);

                if (Filter(learner, fm25Learner))
                {
                    var totalPlannedHours = (learner.PlanLearnHoursNullable ?? 0) + (learner.PlanEEPHoursNullable ?? 0);

                    models.Add(new SummaryOfFundingByStudentReportModel()
                    {
                        Learner = learner,
                        FM25Learner = fm25Learner,
                        TotalPlannedHours = totalPlannedHours,
                    });
                }
            }

            return Order(models);
        }

        public override bool Filter(ILearner learner, FM25Learner fm25Learner)
        {
            return learner != null 
                   && fm25Learner != null
                   && FilterFundLine(fm25Learner.FundLine)
                   && learner.LearningDeliveries?.Any(ld =>
                       ld.FundModel == FundModelConstants.FM25 &&
                       ld.LearningDeliveryFAMs?.Any(FilterSOF) == true) == true;
        }
    }
}
