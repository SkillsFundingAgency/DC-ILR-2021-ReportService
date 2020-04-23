using System;
using System.Collections.Generic;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.ALLB
{
    public class AllbOccupancyReport : AbstractCsvReport<AllbOccupancyReportModel, AllbOccupancyReportClassMap>, IReport
    {
        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.ValidIlr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.Fm99,
        };

        public AllbOccupancyReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<AllbOccupancyReportModel>> modelBuilder,
            ICsvFileService csvService)
             : base(fileNameService,
                 modelBuilder,
                 csvService,
                 ReportTaskNameConstants.AllbOccupancyReport,
                 ReportNameConstants.AllbOccupancy)
        {
        }
    }
}
