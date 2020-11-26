using System;
using System.Collections.Generic;
using CsvHelper.Configuration;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract
{
    public class AbstractSixteenToNineteenReport<TModel, TClassMap> : AbstractCsvReport<TModel, TClassMap> , IReport
        where TClassMap : ClassMap<TModel>
    {
        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.ValidIlr,
            DependentDataCatalog.Fm25,
        };

        public AbstractSixteenToNineteenReport(IFileNameService fileNameService, IModelBuilder<IEnumerable<TModel>> modelBuilder, ICsvFileService csvService, string taskName, string fileName)
            : base(fileNameService, modelBuilder, csvService, taskName, fileName)
        {
        }
    }
}
