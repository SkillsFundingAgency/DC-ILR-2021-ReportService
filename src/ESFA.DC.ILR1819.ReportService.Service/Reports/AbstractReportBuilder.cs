using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public abstract class AbstractReportBuilder
    {
        public void BuildReport<TMapper, TModel>(Stream writer, IEnumerable<TModel> records)
            where TMapper : ClassMap
        {
            using (TextWriter textWriter = new StreamWriter(writer))
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

        public void BuildReport<TMapper, TModel>(Stream writer, TModel record)
            where TMapper : ClassMap
        {
            using (TextWriter textWriter = new StreamWriter(writer))
            {
                using (CsvWriter csvWriter = new CsvWriter(textWriter))
                {
                    csvWriter.Configuration.RegisterClassMap<TMapper>();
                    csvWriter.WriteHeader<TModel>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecord(record);
                }
            }
        }
    }
}
