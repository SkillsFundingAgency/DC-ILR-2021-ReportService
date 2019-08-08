using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration;
using ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive
{
    public class TrailblazerEmployerIncentivesReportClassMap : ClassMap<TrailblazerEmployerIncentivesReportModel>
    {
        public TrailblazerEmployerIncentivesReportClassMap()
        {
            int index = 0;

            Map(m => m.EmployerIdentifier).Name(@"Employer identifier").Index(++index);

            Map(m => m.AugustSmallEmployerIncentive).Name(@"August small employer incentive (£)").Index(++index);
            Map(m => m.August1618ApprenticeIncentive).Name(@"August 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.AugustAchievementPayment).Name(@"August achievement incentive (£)").Index(++index);
            Map(m => m.AugustTotal).Name(@"August total (£)").Index(++index);

            Map(m => m.SeptemberSmallEmployerIncentive).Name(@"September small employer incentive (£)").Index(++index);
            Map(m => m.September1618ApprenticeIncentive).Name(@"September 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.SeptemberAchievementPayment).Name(@"September achievement incentive (£)").Index(++index);
            Map(m => m.SeptemberTotal).Name(@"September total (£)").Index(++index);

            Map(m => m.OctoberSmallEmployerIncentive).Name(@"October small employer incentive (£)").Index(++index);
            Map(m => m.October1618ApprenticeIncentive).Name(@"October 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.OctoberAchievementPayment).Name(@"October achievement incentive (£)").Index(++index);
            Map(m => m.OctoberTotal).Name(@"October total (£)").Index(++index);

            Map(m => m.NovemberSmallEmployerIncentive).Name(@"November small employer incentive (£)").Index(++index);
            Map(m => m.November1618ApprenticeIncentive).Name(@"November 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.NovemberAchievementPayment).Name(@"November achievement incentive (£)").Index(++index);
            Map(m => m.NovemberTotal).Name(@"November total (£)").Index(++index);

            Map(m => m.DecemberSmallEmployerIncentive).Name(@"December small employer incentive (£)").Index(++index);
            Map(m => m.December1618ApprenticeIncentive).Name(@"December 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.DecemberAchievementPayment).Name(@"December achievement incentive (£)").Index(++index);
            Map(m => m.DecemberTotal).Name(@"December total (£)").Index(++index);

            Map(m => m.JanuarySmallEmployerIncentive).Name(@"January small employer incentive (£)").Index(++index);
            Map(m => m.January1618ApprenticeIncentive).Name(@"January 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.JanuaryAchievementPayment).Name(@"January achievement incentive (£)").Index(++index);
            Map(m => m.JanuaryTotal).Name(@"January total (£)").Index(++index);

            Map(m => m.FebruarySmallEmployerIncentive).Name(@"February small employer incentive (£)").Index(++index);
            Map(m => m.February1618ApprenticeIncentive).Name(@"February 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.FebruaryAchievementPayment).Name(@"February achievement incentive (£)").Index(++index);
            Map(m => m.FebruaryTotal).Name(@"February total (£)").Index(++index);

            Map(m => m.MarchSmallEmployerIncentive).Name(@"March small employer incentive (£)").Index(++index);
            Map(m => m.March1618ApprenticeIncentive).Name(@"March 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.MarchAchievementPayment).Name(@"March achievement incentive (£)").Index(++index);
            Map(m => m.MarchTotal).Name(@"March total (£)").Index(++index);

            Map(m => m.AprilSmallEmployerIncentive).Name(@"April small employer incentive (£)").Index(++index);
            Map(m => m.April1618ApprenticeIncentive).Name(@"April 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.AprilAchievementPayment).Name(@"April achievement incentive (£)").Index(++index);
            Map(m => m.AprilTotal).Name(@"April total (£)").Index(++index);

            Map(m => m.MaySmallEmployerIncentive).Name(@"May small employer incentive (£)").Index(++index);
            Map(m => m.May1618ApprenticeIncentive).Name(@"May 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.MayAchievementPayment).Name(@"May achievement incentive (£)").Index(++index);
            Map(m => m.MayTotal).Name(@"May total (£)").Index(++index);

            Map(m => m.JuneSmallEmployerIncentive).Name(@"June small employer incentive (£)").Index(++index);
            Map(m => m.June1618ApprenticeIncentive).Name(@"June 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.JuneAchievementPayment).Name(@"June achievement incentive (£)").Index(++index);
            Map(m => m.JuneTotal).Name(@"June total (£)").Index(++index);

            Map(m => m.JulySmallEmployerIncentive).Name(@"July small employer incentive (£)").Index(++index);
            Map(m => m.July1618ApprenticeIncentive).Name(@"July 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.JulyAchievementPayment).Name(@"July achievement incentive (£)").Index(++index);
            Map(m => m.JulyTotal).Name(@"July total (£)").Index(++index);

            Map(m => m.TotalSmallEmployerIncentive).Name(@"Total small employer incentive (£)").Index(++index);
            Map(m => m.Total1618ApprenticeIncentive).Name(@"Total 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.TotalAchievementPayment).Name(@"Total achievement incentive (£)").Index(++index);
            Map(m => m.GrandTotal).Name(@"Grand total (£)").Index(++index);
        }
    }
}
