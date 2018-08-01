using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public abstract class AbstractReportBuilder
    {
        protected void BuildReport<TMapper, TModel>(MemoryStream writer, IEnumerable<TModel> records)
            where TMapper : ClassMap, IClassMapper
            where TModel : class
        {
            using (TextWriter textWriter = new StreamWriter(writer, Encoding.UTF8, 1024, true))
            {
                using (CsvWriter csvWriter = new CsvWriter(textWriter))
                {
                    csvWriter.Configuration.RegisterClassMap<TMapper>();
                    csvWriter.WriteHeader<TModel>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecords(records);
                }
            }
        }

        protected void BuildReport<TMapper, TModel>(MemoryStream writer, TModel record)
            where TMapper : ClassMap, IClassMapper
            where TModel : class
        {
            BuildReport<TMapper, TModel>(writer, new[] { record });
        }
    }
}
