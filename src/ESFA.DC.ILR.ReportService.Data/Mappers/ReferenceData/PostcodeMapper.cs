using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Postcodes;
using ESFA.DC.ILR.ReportService.Service.Interface.Mappers.ReferenceData;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class PostcodeMapper : IPostcodeMapper
    {
        public IReadOnlyCollection<Postcode> MapData(IEnumerable<ReferenceDataService.Model.Postcodes.Postcode> postcodes)
        {
            return postcodes?.Select(MapPostcode).ToList();
        }

        private Postcode MapPostcode(ReferenceDataService.Model.Postcodes.Postcode postcode)
        {
            return new Postcode()
            {
                PostCode = postcode.PostCode,
                ONSData = postcode.ONSData?.Select(MapOnsData).ToList(),
                DasDisadvantages = postcode.DasDisadvantages?.Select(MapDasDisadvantage).ToList(),
                EfaDisadvantages = postcode.EfaDisadvantages?.Select(MapEfaDisadvantage).ToList(),
                SfaAreaCosts = postcode.SfaAreaCosts?.Select(MapSfaAreaCost).ToList(),
                SfaDisadvantages = postcode.SfaDisadvantages?.Select(MapSfaDisadvantage).ToList(),
                McaglaSOFs = postcode.McaglaSOFs?.Select(MapMcaglaSof).ToList()
            };
        }

        private ONSData MapOnsData(ReferenceDataService.Model.Postcodes.ONSData onsData)
        {
            return new ONSData()
            {
                Termination = onsData.Termination,
                LocalAuthority = onsData.LocalAuthority,
                Lep1 = onsData.Lep1,
                Lep2 = onsData.Lep2,
                Nuts = onsData.Nuts,
                EffectiveTo = onsData.EffectiveTo,
                EffectiveFrom = onsData.EffectiveFrom
            };
        }

        private DasDisadvantage MapDasDisadvantage(ReferenceDataService.Model.Postcodes.DasDisadvantage dasDisadvantage)
        {
            return new DasDisadvantage()
            {
                Uplift = dasDisadvantage.Uplift,
                EffectiveTo = dasDisadvantage.EffectiveTo,
                EffectiveFrom = dasDisadvantage.EffectiveFrom
            };
        }

        private EfaDisadvantage MapEfaDisadvantage(ReferenceDataService.Model.Postcodes.EfaDisadvantage efaDisadvantage)
        {
            return new EfaDisadvantage()
            {
                Uplift = efaDisadvantage.Uplift,
                EffectiveTo = efaDisadvantage.EffectiveTo,
                EffectiveFrom = efaDisadvantage.EffectiveFrom
            };
        }

        private SfaAreaCost MapSfaAreaCost(ReferenceDataService.Model.Postcodes.SfaAreaCost sfaAreaCost)
        {
            return new SfaAreaCost()
            {
                AreaCostFactor = sfaAreaCost.AreaCostFactor,
                EffectiveTo = sfaAreaCost.EffectiveTo,
                EffectiveFrom = sfaAreaCost.EffectiveFrom
            };
        }

        private SfaDisadvantage MapSfaDisadvantage(ReferenceDataService.Model.Postcodes.SfaDisadvantage sfaDisadvantage)
        {
            return new SfaDisadvantage()
            {
                Uplift = sfaDisadvantage.Uplift,
                EffectiveTo = sfaDisadvantage.EffectiveTo,
                EffectiveFrom = sfaDisadvantage.EffectiveFrom
            };
        }

        private McaglaSOF MapMcaglaSof(ReferenceDataService.Model.Postcodes.McaglaSOF mcaglaSof)
        {
            return new McaglaSOF()
            {
                SofCode = mcaglaSof.SofCode,
                EffectiveTo = mcaglaSof.EffectiveTo,
                EffectiveFrom = mcaglaSof.EffectiveFrom
            };
        }
    }
}
