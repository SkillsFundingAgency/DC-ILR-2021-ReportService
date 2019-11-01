using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.MathsAndEnglish
{
    public class MathsAndEnglishReportModelBuilder : AbstractSixteenToNineteenReportModelBuilder, IModelBuilder<IEnumerable<MathsAndEnglishReportModel>>
    {
        public IEnumerable<MathsAndEnglishReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm25 = reportServiceDependentData.Get<FM25Global>();

            var fm25LearnerDictionary = BuildFm25LearnerDictionary(fm25);

            var models = new List<MathsAndEnglishReportModel>();

            foreach (var learner in message?.Learners ?? Enumerable.Empty<ILearner>())
            {
                var fm25Learner = fm25LearnerDictionary.GetValueOrDefault(learner.LearnRefNumber);

                if (Filter(learner, fm25Learner))
                {
                    models.Add(new MathsAndEnglishReportModel()
                    {
                        Learner = learner,
                        FM25Learner = fm25Learner,
                    });
                }
            }

            return Order(models);
        }
    }
}
