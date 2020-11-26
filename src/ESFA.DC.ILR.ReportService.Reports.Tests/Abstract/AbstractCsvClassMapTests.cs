using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Abstract
{
    public abstract class AbstractCsvClassMapTests<TModel, TClassMap> where TClassMap : ClassMap
    {
        protected abstract IEnumerable<string> OrderedColumns { get; }

        [Fact]
        public void Map_Columns()
        {
            var input = new List<TModel>();

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<TClassMap>();

                        csvWriter.WriteRecords(input);
                    }
                }

                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    using (var csvReader = new CsvReader(streamReader))
                    {
                        csvReader.Read();
                        csvReader.ReadHeader();
                        var header = csvReader.Context.HeaderRecord;

                        header.Should().ContainInOrder(OrderedColumns);

                        header.Should().HaveCount(OrderedColumns.Count());
                    }
                }
            }
        }
        
        protected IEnumerable<dynamic> WriteAndReadModel(IEnumerable<TModel> models)
        {        
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<TClassMap>();
                        csvWriter.Configuration.TypeConverterOptionsCache.GetOptions<System.DateTime?>().Formats = new[] { "dd/MM/yyyy" };
                        csvWriter.Configuration.TypeConverterOptionsCache.GetOptions<System.DateTime>().Formats = new[] { "dd/MM/yyyy" };

                        csvWriter.WriteRecords(models);
                    }
                }

                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    using (var csvReader = new CsvReader(streamReader))
                    {
                        return csvReader.GetRecords<dynamic>().ToList();
                    }
                }
            }
        }
    }
}
