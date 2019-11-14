using System.Dynamic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Models.Ilr
{
    public class MessageHeader : IHeader
    {
        public ICollectionDetails CollectionDetailsEntity { get; set; }

        public ISource SourceEntity => throw new System.NotImplementedException();
    }
}
