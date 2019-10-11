using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Summary
{
    public class RuleViolationSummaryReportModelBuilder : AbstractReportModelBuilder, IModelBuilder<RuleViolationSummaryReportModel>
    {
        public RuleViolationSummaryReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            throw new NotImplementedException();
        }
    }
}
