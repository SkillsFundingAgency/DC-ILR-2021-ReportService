using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Model
{
    public class PeriodisedValuesLookup : Dictionary<FundModels, Dictionary<string, Dictionary<string, decimal?[][]>>>, IPeriodisedValuesLookup
    {
        public IEnumerable<decimal?[]> GetPeriodisedValues(FundModels fundModel, IEnumerable<string> fundLines, IEnumerable<string> attributes)
        {
            var periodisedValuesList = new List<decimal?[]>();

            try
            {
                var fundLineDictionary = this[fundModel];

                foreach (var fundLine in fundLines)
                {
                    foreach (var attribute in attributes)
                    {
                        var attributesDictionary = fundLineDictionary[fundLine];

                        try
                        {
                            periodisedValuesList.AddRange(attributesDictionary[attribute]);
                        }
                        catch (KeyNotFoundException)
                        {
                        }
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                return null;
            }

            return periodisedValuesList;
        }
    }
}
