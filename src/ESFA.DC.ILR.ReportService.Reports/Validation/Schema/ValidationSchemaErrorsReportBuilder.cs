using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Schema
{
    public class ValidationSchemaErrorsReportBuilder : IValidationSchemaErrorsReportBuilder
    {
        private readonly Dictionary<string, string> _errorMessageLookup = new Dictionary<string, string>
        {
            { "Entity_1", "You're unable to submit this file because it doesn't contain any valid learners.Please upload an updated file" },
            { "Filename_1", "There's a problem. The filename should use the format ILR-LLLLLLLL-YYYY-yyyymmdd-hhmmss-NN.xml" },
            { "Filename_2", "There's a problem.  You have already uploaded a file with the same filename. Upload a file with a different filename." },
            { "Filename_4", "There's a problem. The UK Provider Reference Number (UKPRN) is not valid - check the UKPRN is correct." },
            { "Filename_5", "There's a problem.  The year in the filename should match the current year." },
            { "Filename_6", "The serial number in the filename(the last two characters) must be a two digit number." },
            { "Filename_7", "There's a problem. The serial number in the filename must not be 00." },
            { "Filename_8", "There's a problem.  The date/time in your filename must not be in the future." },
            { "Filename_9", "There's a problem. The date and time in the filename must not be earlier than a file already uploaded." },
            { "Header_3 ", "You're unable to submit this file because the UKPRN in the file's header record doesn't match the UKPRN in the filename. Please upload an updated file." },
            { "Inconsistent UKPRN", "You're unable to submit this file because the UKPRN in the file's header record doesn't match the UKPRN in the filename. Please upload an updated file." },
            { "Namespace Mismatch", "You're unable to submit this file because the namespace doesn't match the namespace outlined in the schema.You can download the ILR schema definitions from the ILR data guidance page [link to https://www.gov.uk/government/collections/individualised-learner-record-ilr] for the relevant academic year. Please upload an updated file." },
            { "Protected ZIP File", "ZIP file is password protected and cannot be processed." },
            { "Schema", "The XML is not well formed." },
            { "UKPRN_03", "The UKPRN is not the same as recorded in the Header." },
            { "ZIP_CORRUPT", "Zip folder is corrupt or invalid." },
            { "ZIP_EMPTY", "Zip folder must contain only one XML file." },
            { "ZIP_INCONSISTENT_FILENAME", "Zip filename does not match the xml filename." }
        };

    public IEnumerable<ValidationErrorRow> Build(IEnumerable<ValidationError> ilrValidationErrors)
        {
            return ilrValidationErrors
                .Select(e => new ValidationErrorRow()
                {
                    AimSequenceNumber = e.AimSequenceNumber,
                    ErrorMessage = _errorMessageLookup.GetValueOrDefault(e.RuleName),
                    FieldValues = e.ValidationErrorParameters == null
                        ? string.Empty
                        : GetValidationErrorParameters(e.ValidationErrorParameters),
                    LearnerReferenceNumber = e.LearnerReferenceNumber,
                    RuleName = e.RuleName,
                    Severity = e.Severity
                })
                .OrderBy(e => e.Severity)
                .ThenBy(e => e.RuleName);
        }

        private string GetValidationErrorParameters(IEnumerable<ValidationErrorParameter> validationErrorParameters)
        {
            StringBuilder result = new StringBuilder();

            foreach (var parameter in validationErrorParameters)
            {
                result.Append($"{parameter.PropertyName}={parameter.Value}|");
            }
          
            return result.ToString();
        }
    }
}
