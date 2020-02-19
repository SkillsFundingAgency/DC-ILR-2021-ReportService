using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.NonContractDevolved
{
    public class NonContractDevolvedAdultEducationOccupancyReport : AbstractCsvReport<NonContractDevolvedAdultEducationOccupancyReportModel, NonContractDevolvedAdultEducationOccupancyReportClassMap>, IReport
    {
        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.ValidIlr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.Fm35,
        };

        public NonContractDevolvedAdultEducationOccupancyReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<NonContractDevolvedAdultEducationOccupancyReportModel>> nonContractDevolvedAdultEducationOccupancyReportModelBuilder,
            ICsvService csvService)
            : base(
                fileNameService,
                nonContractDevolvedAdultEducationOccupancyReportModelBuilder,
                csvService,
                ReportTaskNameConstants.NonContractDevolvedAdultEducationOccupancyReport,
                "Non-Contracted Devolved Adult Education Activity Report")
        {
        }
    }
}
