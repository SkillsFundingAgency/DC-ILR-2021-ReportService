using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.ILR.ValidationErrors.Model;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.Poco;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Service
{
    public sealed class ValidationErrorsService : IValidationErrorsService
    {
        private ILogger _logger;
        private readonly IReportServiceConfiguration _reportServiceConfiguration;

        public ValidationErrorsService(
            ILogger logger,
            IReportServiceConfiguration reportServiceConfiguration)
        {
            _logger = logger;
            _reportServiceConfiguration = reportServiceConfiguration;
        }

        public async Task PopulateValidationErrors(string[] ruleNames, List<ValidationErrorDetails> validationErrors, CancellationToken cancellationToken)
        {
            try
            {
                using (Data.ILR.ValidationErrors.Model.ValidationErrors validationErrorsContext =
                    new Data.ILR.ValidationErrors.Model.ValidationErrors(_reportServiceConfiguration.IlrValidationErrorsConnectionString))
                {
                    List<Rule> errors = await validationErrorsContext
                            .Rules
                            .Where(x => ruleNames.Contains(x.Rulename))
                            .ToListAsync(cancellationToken);

                    foreach (ValidationErrorDetails validationError in validationErrors)
                    {
                        Rule rule = errors.SingleOrDefault(x => string.Equals(x.Rulename, validationError.RuleName, StringComparison.OrdinalIgnoreCase));
                        if (rule != null)
                        {
                            validationError.Message = rule.Message;
                            validationError.Severity = rule.Severity;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to retrieve validation errors", ex);
            }
        }
    }
}
