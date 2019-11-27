using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using ESFA.DC.ILR.ReportService.Models.EAS;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class EasProvider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public EasProvider(
            IFileService fileService, 
            ISerializationService serializationService) : base(fileService, serializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var referenceData = await ProvideAsync<ReferenceDataRoot>(reportServiceContext.IlrReferenceDataKey, reportServiceContext.Container, cancellationToken) as ReferenceDataRoot;

            var easFundingLines = referenceData?.EasFundingLines;

            return MapData(easFundingLines);
        }

        private IReadOnlyCollection<EasFundingLine> MapData(IEnumerable<ReferenceDataService.Model.EAS.EasFundingLine> fundingLine)
        {
            return fundingLine?.Select(MapFundingLine).ToList();
        }

        private EasFundingLine MapFundingLine(ReferenceDataService.Model.EAS.EasFundingLine fundingLine)
        {
            return new EasFundingLine()
            {
                FundLine = fundingLine.FundLine,
                EasSubmissionValues = fundingLine.EasSubmissionValues?.Select(MapEasSubmissionValue).ToList()
            };
        }

        private EasSubmissionValue MapEasSubmissionValue(ReferenceDataService.Model.EAS.EasSubmissionValue easSubmissionValue)
        {
            return new EasSubmissionValue()
            {
                PaymentName = easSubmissionValue.PaymentName,
                AdjustmentTypeName = easSubmissionValue.AdjustmentTypeName,
                Period1 = easSubmissionValue.Period1?.Select(MapEasPaymentValue).ToList(),
                Period2 = easSubmissionValue.Period2?.Select(MapEasPaymentValue).ToList(),
                Period3 = easSubmissionValue.Period3?.Select(MapEasPaymentValue).ToList(),
                Period4 = easSubmissionValue.Period4?.Select(MapEasPaymentValue).ToList(),
                Period5 = easSubmissionValue.Period5?.Select(MapEasPaymentValue).ToList(),
                Period6 = easSubmissionValue.Period6?.Select(MapEasPaymentValue).ToList(),
                Period7 = easSubmissionValue.Period7?.Select(MapEasPaymentValue).ToList(),
                Period8 = easSubmissionValue.Period8?.Select(MapEasPaymentValue).ToList(),
                Period9 = easSubmissionValue.Period9?.Select(MapEasPaymentValue).ToList(),
                Period10 = easSubmissionValue.Period10?.Select(MapEasPaymentValue).ToList(),
                Period11 = easSubmissionValue.Period11?.Select(MapEasPaymentValue).ToList(),
                Period12 = easSubmissionValue.Period12?.Select(MapEasPaymentValue).ToList()
            };
        }

        private EasPaymentValue MapEasPaymentValue(ReferenceDataService.Model.EAS.EasPaymentValue easPaymentValue)
        {
            return new EasPaymentValue(easPaymentValue.PaymentValue, easPaymentValue.DevolvedAreaSofs);
        }
    }
}
