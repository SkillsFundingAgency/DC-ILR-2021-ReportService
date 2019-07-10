using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Model.ILR;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Service.Provider
{
    public sealed class IlrMetadataProviderService : IIlrMetadataProviderService
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly Func<IIlr1819ValidContext> _ilrValidContextFactory;
        private readonly Func<IIlr1819RulebaseContext> _ilrRulebaseContextFactory;

        public IlrMetadataProviderService(
            IDateTimeProvider dateTimeProvider,
            Func<IIlr1819ValidContext> ilrValidContextFactory,
            Func<IIlr1819RulebaseContext> ilrRulebaseContextFactory)
        {
            _dateTimeProvider = dateTimeProvider;
            _ilrValidContextFactory = ilrValidContextFactory;
            _ilrRulebaseContextFactory = ilrRulebaseContextFactory;
        }

       public async Task<ILRSourceFileInfo> GetLastSubmittedIlrFile(
           IReportServiceContext reportServiceContext,
           CancellationToken cancellationToken)
        {
            var ilrFileDetail = new ILRSourceFileInfo();

            cancellationToken.ThrowIfCancellationRequested();

            var ukPrn = reportServiceContext.Ukprn;
            using (var ilrContext = _ilrRulebaseContextFactory())
            {
                var fileDetail = await ilrContext.FileDetails.Where(x => x.UKPRN == ukPrn).OrderByDescending(x => x.ID).FirstOrDefaultAsync(cancellationToken);
                if (fileDetail != null)
                {
                    var filename = fileDetail.Filename.Contains('/') ? fileDetail.Filename.Split('/')[1] : fileDetail.Filename;

                    ilrFileDetail.UKPRN = fileDetail.UKPRN;
                    ilrFileDetail.Filename = filename;
                    ilrFileDetail.SubmittedTime = fileDetail.SubmittedTime;
                }
            }

            using (var ilrContext = _ilrValidContextFactory())
            {
                var collectionDetail = await ilrContext.CollectionDetails.FirstOrDefaultAsync(x => x.UKPRN == ukPrn, cancellationToken);
                if (collectionDetail != null)
                {
                    ilrFileDetail.FilePreparationDate = collectionDetail.FilePreparationDate;
                }
            }

            return ilrFileDetail;
        }
    }
}