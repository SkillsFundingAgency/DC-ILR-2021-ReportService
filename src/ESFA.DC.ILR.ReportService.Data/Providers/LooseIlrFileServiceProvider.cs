using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model.Loose;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class LooseIlrFileServiceProvider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public LooseIlrFileServiceProvider(IFileService fileService, IXmlSerializationService xmlSerializationService)
        : base(fileService, xmlSerializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var message = await ProvideAsync<Message>(reportServiceContext.OriginalFilename, reportServiceContext.Container, cancellationToken) as Message;
            var invalidLearnRefNumbers = await ProvideAsync<List<string>>(reportServiceContext.InvalidLearnRefNumbersKey, reportServiceContext.Container, cancellationToken) as List<string>;

            message.Learner = message.Learner.Where(l => invalidLearnRefNumbers.Contains(l.LearnRefNumber)).ToArray();
            message.LearnerDestinationandProgression = message.LearnerDestinationandProgression.Where(l => invalidLearnRefNumbers.Contains(l.LearnRefNumber)).ToArray();

            return message;
        }
    }
}
