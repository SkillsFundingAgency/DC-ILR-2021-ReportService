using CsvHelper.Configuration;

namespace ESFA.DC.ILR1819.ReportService.Tests.Models
{
    public sealed class CsvEntry
    {
        public CsvEntry(ClassMap mapper, int dataRows)
        {
            Mapper = mapper;
            DataRows = dataRows;
        }

        public ClassMap Mapper { get; }

        public int DataRows { get; }
    }
}
