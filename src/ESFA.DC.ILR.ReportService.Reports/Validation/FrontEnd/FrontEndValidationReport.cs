﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.FrontEnd
{
    public class FrontEndValidationReport : IFrontEndValidationReport
    {
        private readonly IFileService _fileService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IFileNameService _fileNameService;
        private readonly ILogger _logger;
        

        public FrontEndValidationReport(IFileService fileService, IJsonSerializationService jsonSerializationService, IFileNameService fileNameService, ILogger logger)
        {
            _fileService = fileService;
            _jsonSerializationService = jsonSerializationService;
            _fileNameService = fileNameService;
            _logger = logger;
        }

        public async Task GenerateAsync(IReportServiceContext reportServiceContext, IEnumerable<ValidationErrorRow> validationErrorDtos, bool isSchemaError, CancellationToken cancellationToken)
        {
            var validationErrorDtosList = validationErrorDtos.ToList();

            var errors = validationErrorDtosList.Where(x =>
                x.Severity.CaseInsensitiveEquals("E") ||
                x.Severity.CaseInsensitiveEquals("F"))
                .ToArray();

            var warnings = validationErrorDtosList
                .Where(x => x.Severity.CaseInsensitiveEquals("W")).ToArray();

            var ilrValidationResult = new FileValidationResult
            {
                TotalLearners = GetNumberOfLearners(reportServiceContext),
                TotalErrors = errors.Length,
                TotalWarnings = warnings.Length,
                TotalWarningLearners = warnings.DistinctByCount(x => x.LearnerReferenceNumber?.Trim()),
                TotalErrorLearners = errors.DistinctByCount(x => x.LearnerReferenceNumber?.Trim()),
                ErrorMessage = validationErrorDtosList
                    .FirstOrDefault(x => x.Severity.CaseInsensitiveEquals("F"))
                    ?.ErrorMessage,
                IsSchemaError = isSchemaError
            };

            var fileName = _fileNameService.GetFilename(reportServiceContext, "Rule Violation Report", OutputTypes.Json);

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
