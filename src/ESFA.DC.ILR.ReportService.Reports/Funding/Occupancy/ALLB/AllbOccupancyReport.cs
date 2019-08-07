using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

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
            ICsvService csvService)
             : base(fileNameService,
                 modelBuilder,
                 csvService,
                 ReportTaskNameConstants.AllbOccupancyReport,
                 "ALLB Occupancy Report")
        {
        }
    }
}
