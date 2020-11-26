using System;
using System.Collections.Generic;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Trailblazer
{
    public class TrailblazerOccupancyReport : AbstractCsvReport<TrailblazerOccupancyReportModel, TrailblazerOccupancyReportClassMap>, IReport
    {
        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.ValidIlr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.Fm81,
        };

        public TrailblazerOccupancyReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<TrailblazerOccupancyReportModel>> modelBuilder,
            ICsvFileService csvService)
            : base(fileNameService,
                modelBuilder,
                csvService,
                ReportTaskNameConstants.TrailblazerAppsOccupancyReport,
                ReportNameConstants.TrailblazerOccupancy)
        {
        }

    }
}
