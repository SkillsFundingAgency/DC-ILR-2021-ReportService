using System;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Models.Ilr
{
    public class MessageHeaderCollectionDetails : ICollectionDetails
    {
        public DateTime FilePreparationDate { get; set; }

        public string CollectionString => throw new NotImplementedException();

        public string YearString => throw new NotImplementedException();
    }
}
