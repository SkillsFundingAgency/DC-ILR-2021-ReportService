using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.FrontEnd
{
    public class FrontEndValidationReport : IFrontEndValidationReport
    {
        private readonly IFileService _fileService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly ILogger _logger;

        public FrontEndValidationReport(IFileService fileService, IJsonSerializationService jsonSerializationService, ILogger logger)
        {
            _fileService = fileService;
            _jsonSerializationService = jsonSerializationService;
            _logger = logger;
        }

        public async Task GenerateAsync(IReportServiceContext reportServiceContext, IEnumerable<ValidationErrorDto> validationErrorDtos, string externalFileName, CancellationToken cancellationToken)
        {
            var validationErrorDtosList = validationErrorDtos.ToList();

            var errors = validationErrorDtosList.Where(x =>
                string.Equals(x.Severity, "E", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Severity, "F", StringComparison.OrdinalIgnoreCase)).ToArray();
            var warnings = validationErrorDtosList
                .Where(x => string.Equals(x.Severity, "W", StringComparison.OrdinalIgnoreCase)).ToArray();

            var ilrValidationResult = new FileValidationResult
            {
                TotalLearners = GetNumberOfLearners(reportServiceContext),
                TotalErrors = errors.Length,
                TotalWarnings = warnings.Length,
                TotalWarningLearners = warnings.DistinctByCount(x => x.LearnerReferenceNumber),
                TotalErrorLearners = errors.DistinctByCount(x => x.LearnerReferenceNumber),
                ErrorMessage = validationErrorDtosList
                    .FirstOrDefault(x => string.Equals(x.Severity, "F", StringComparison.OrdinalIgnoreCase))
                    ?.ErrorMessage,
                //TotalDataMatchErrors = _validationStageOutputCache.DataMatchProblemCount,
                //TotalDataMatchLearners = _validationStageOutputCache.DataMatchProblemLearnersCount
            };

            var fileName = $"{externalFileName}.json";

            using (var fileStream = await _fileService.OpenWriteStreamAsync(fileName, reportServiceContext.Container, cancellationToken))
            {
                _jsonSerializationService.Serialize(ilrValidationResult, fileStream);
            }
        }

        private int GetNumberOfLearners(IReportServiceContext reportServiceContext)
        {
            int ret = 0;
            try
            {
                ret = reportServiceContext.ValidLearnRefNumbersCount;
                ret = ret + reportServiceContext.InvalidLearnRefNumbersCount;
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't read number of learners", ex);
            }

            return ret;
        }
    }
}
