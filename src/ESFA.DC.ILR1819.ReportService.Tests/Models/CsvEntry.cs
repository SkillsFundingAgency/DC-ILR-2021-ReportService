using CsvHelper.Configuration;

namespace ESFA.DC.ILR1819.ReportService.Tests.Models
{
    public sealed class CsvEntry
    {
        public CsvEntry(ClassMap mapper, int dataRows, int blankRowsBefore = 0)
        {
            Mapper = mapper;
            DataRows = dataRows;
            BlankRowsBefore = blankRowsBefore;
        }

        public ClassMap Mapper { get; }

        public int DataRows { get; }

        public int BlankRowsBefore { get; }
    }
}
