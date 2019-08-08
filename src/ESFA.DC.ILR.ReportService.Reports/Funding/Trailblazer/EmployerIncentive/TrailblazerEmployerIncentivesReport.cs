using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive
{
    public class TrailblazerEmployerIncentivesReport : AbstractCsvReport<TrailblazerEmployerIncentivesReportModel, TrailblazerEmployerIncentivesReportClassMap>, IReport
    {
        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.Fm81
        };

        public TrailblazerEmployerIncentivesReport(
            IFileNameService fileNameService, 
            IModelBuilder<IEnumerable<TrailblazerEmployerIncentivesReportModel>> modelBuilder, 
            ICsvService csvService) 
            : base(fileNameService, modelBuilder, csvService, ReportTaskNameConstants.TrailblazerEmployerIncentivesReport, "Trailblazer Apprenticeships Employer Incentives Report")
        {
        }
    }
}
