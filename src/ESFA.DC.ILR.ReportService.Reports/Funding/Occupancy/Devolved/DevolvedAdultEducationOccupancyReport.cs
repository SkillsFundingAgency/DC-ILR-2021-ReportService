using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved
{
    public class DevolvedAdultEducationOccupancyReport : AbstractCsvReport<DevolvedAdultEducationOccupancyReportModel, DevolvedAdultEducationOccupancyReportClassMap>, IReport
    {
        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.ValidIlr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.Fm35,
        };

        public DevolvedAdultEducationOccupancyReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>> devolvedAdultEducationOccupancyReportModelBuilder,
            ICsvService csvService)
            : base(
                fileNameService,
                devolvedAdultEducationOccupancyReportModelBuilder,
                csvService,
                ReportTaskNameConstants.DevolvedAdultEducationOccupancyReport,
                "Devolved Adult Education Occupancy Report")
        {
        }
    }
}
