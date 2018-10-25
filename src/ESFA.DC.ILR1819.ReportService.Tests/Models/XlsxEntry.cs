using CsvHelper.Configuration;

namespace ESFA.DC.ILR1819.ReportService.Tests.Models
{
    public sealed class XlsxEntry
    {
        public XlsxEntry(ClassMap mapper, int dataRows, bool pivot = false)
        {
            Mapper = mapper;
            DataRows = dataRows;
            Pivot = pivot;
        }

        public ClassMap Mapper { get; }

        public int DataRows { get; }

        public bool Pivot { get; }
    }
}
