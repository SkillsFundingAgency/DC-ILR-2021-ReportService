using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Models.Ilr
{
    public class MessageHeader : IHeader
    {
        public MessageHeaderCollectionDetails CollectionDetails { get; set; }

        public ICollectionDetails CollectionDetailsEntity => throw new System.NotImplementedException();

        public ISource SourceEntity => throw new System.NotImplementedException();
    }
}
