using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider
{
    public class FM35Provider : AbstractFundModelProviderService, IFM35ProviderService
    {
        private readonly SemaphoreSlim _getDataLock = new SemaphoreSlim(1, 1);
        private bool _loadedDataAlready;
        private FM35Global _fundingOutputs;

        public FM35Provider(
            ILogger logger,
            IFileService fileService,
            IJsonSerializationService jsonSerializationService)
            : base(fileService, jsonSerializationService, logger)
        {
        }

        public async Task<FM35Global> GetFM35Data(
            IReportServiceContext reportServiceContext,
            CancellationToken cancellationToken)
        {
            await _getDataLock.WaitAsync(cancellationToken);

            try
            {
                if (_loadedDataAlready)
                {
                    return _fundingOutputs;
                }

                cancellationToken.ThrowIfCancellationRequested();
                
                _fundingOutputs = await Provide<FM35Global>(reportServiceContext.FundingFM35OutputKey, reportServiceContext.Container, cancellationToken);

                _loadedDataAlready = true;
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }
    }
}