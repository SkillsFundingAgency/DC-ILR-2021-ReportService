using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy.Model;
using ESFA.DC.ILR.ReportService.Service.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy
{
    public class DevolvedAdultEducationOccupancyReportModelBuilder : IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>>
    {
        private readonly IEnumerable<string> _sofLearnDelFamCodes = new HashSet<string>()
        {
            "110", "111", "112", "113", "114", "115", "116"
        };

        public IEnumerable<DevolvedAdultEducationOccupancyReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm35 = reportServiceDependentData.Get<FM35Global>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();
            
            foreach (var learner in message.Learners)
            {
                foreach (var learningDelivery in learner.LearningDeliveries.Where(LearningDeliveryReportFilter))
                {
                    yield return new DevolvedAdultEducationOccupancyReportModel()
                    {
                        Learner = learner,
                        LearningDelivery = learningDelivery,
                        
                        // fm35 ld
                        // lars LD
                        // devolved
                    };
                }
            }
        }

        public bool LearningDeliveryReportFilter(ILearningDelivery learningDelivery)
        {
            return learningDelivery
                .LearningDeliveryFAMs?
                .Any(
                    ldfam => 
                        ldfam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.SOF)
                           && _sofLearnDelFamCodes.Contains(ldfam.LearnDelFAMCode))
                ?? false;
        }
    }
}
