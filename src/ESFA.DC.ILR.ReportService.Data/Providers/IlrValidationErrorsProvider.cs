using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class IlrValidationErrorsProvider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public IlrValidationErrorsProvider(IFileService fileService, IJsonSerializationService jsonSerializationService)
            : base(fileService, jsonSerializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
            => await ProvideAsync<List<ValidationError>>(reportServiceContext.ValidationErrorsKey, reportServiceContext.Container, cancellationToken);
    }
}
