using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Trailblazer
{
    public class TrailblazerOccupancyReport : AbstractCsvReport<TrailblazerOccupancyReportModel, TrailblazerOccupancyReportClassMap>, IReport
    {
        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.Ilr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.Fm81,
        };

        public TrailblazerOccupancyReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<TrailblazerOccupancyReportModel>> modelBuilder,
            ICsvService csvService)
            : base(fileNameService,
                modelBuilder,
                csvService,
                ReportTaskNameConstants.TrailblazerAppsOccupancyReport,
                "Trailblazer Occupancy Report")
        {
        }

    }
}
