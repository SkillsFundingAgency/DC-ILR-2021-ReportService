namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ESFA.DC.ILR1819.ReportService.Interface.Builders;
    using ESFA.DC.ILR1819.ReportService.Model.Eas;
    using ESFA.DC.ILR1819.ReportService.Model.ILR;
    using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

    public class AdultFundingClaimBuilder : IAdultFundingClaimBuilder
    {
        public AdultFundingClaimModel BuildAdultFundingClaimModel(
            List<FM35LearningDeliveryValues> fm35LearningDeliveryPeriodisedValues,
            List<EasSubmissionValues> easSubmissionValues,
            List<ALBLearningDeliveryValues> albLearningDeliveryPeriodisedValues)
        {
            var adultFundingClaimModel = new AdultFundingClaimModel();

            // FM35
            decimal otherLearningProgramFunding6MonthsFm35 = 0;
            decimal otherLearningProgramFunding12MonthsFm35 = 0;
            decimal otherLearningSupport6MonthsFm35 = 0;
            decimal otherLearningSupport12MonthsFm35 = 0;
            decimal traineeShips1924ProgrammeFunding6MonthsFm35 = 0;
            decimal traineeShips1924ProgrammeFunding12MonthsFm35 = 0;
            decimal traineeShips1924LearningSupport6MonthsFm35 = 0;
            decimal traineeShips1924LearningSupport12MonthsFm35 = 0;

            // EAS
            decimal otherLearningProgramFunding6MonthsEas = 0;
            decimal otherLearningProgramFunding12MonthsEas = 0;
            decimal otherLearningSupport6MonthsEas = 0;
            decimal otherLearningSupport12MonthsEas = 0;
            decimal traineeShips1924ProgrammeFunding6MonthsEas = 0;
            decimal traineeShips1924ProgrammeFunding12MonthsEas = 0;
            decimal traineeShips1924LearningSupport6MonthsEas = 0;
            decimal traineeShips1924LearningSupport12MonthsEas = 0;
            decimal loansAreasCosts6MonthsEas = 0;
            decimal loansAreasCosts12MonthsEas = 0;
            decimal loansExcessSupport6MonthsEas = 0;
            decimal loansExcessSupport12MonthsEas = 0;

            // ALB
            decimal loansBursaryFunding6Months = 0;
            decimal loansBursaryFunding12Months = 0;
            decimal loansAreaCosts6Months = 0;
            decimal loansAreaCosts12Months = 0;

            // FM35
            otherLearningProgramFunding6MonthsFm35 = Fm35DeliveryValues6Months(fm35LearningDeliveryPeriodisedValues, new[] { "OnProgPayment", "BalancePayment", "AchievePayment", "EmpOutcomePay" }, new[] { "AEB – Other Learning", "AEB – Other Learning (non-procured)" });
            otherLearningProgramFunding12MonthsFm35 = Fm35DeliveryValues12Months(fm35LearningDeliveryPeriodisedValues, new[] { "OnProgPayment", "BalancePayment", "AchievePayment", "EmpOutcomePay" }, new[] { "AEB – Other Learning", "AEB – Other Learning (non-procured)" });
            otherLearningSupport6MonthsFm35 = Fm35DeliveryValues6Months(fm35LearningDeliveryPeriodisedValues, new[] { "LearnSuppFundCash" }, new[] { "AEB – Other Learning", "AEB – Other Learning (non-procured)" });
            otherLearningSupport12MonthsFm35 = Fm35DeliveryValues12Months(fm35LearningDeliveryPeriodisedValues, new[] { "LearnSuppFundCash" }, new[] { "AEB – Other Learning", "AEB – Other Learning (non-procured)" });
            traineeShips1924ProgrammeFunding6MonthsFm35 = Fm35DeliveryValues6Months(fm35LearningDeliveryPeriodisedValues, new[] { "OnProgPayment", "BalancePayment", "AchievePayment", "EmpOutcomePay" }, new[] { "19-24 Traineeship’", "19-24 Traineeship (non-procured)" });
            traineeShips1924ProgrammeFunding12MonthsFm35 = Fm35DeliveryValues12Months(fm35LearningDeliveryPeriodisedValues, new[] { "OnProgPayment", "BalancePayment", "AchievePayment", "EmpOutcomePay" }, new[] { "19-24 Traineeship’", "19-24 Traineeship (non-procured)" });
            traineeShips1924LearningSupport6MonthsFm35 = Fm35DeliveryValues6Months(fm35LearningDeliveryPeriodisedValues, new[] { "LearnSuppFundCash" }, new[] { "19-24 Traineeship’", "19-24 Traineeship (non-procured)" });
            traineeShips1924LearningSupport12MonthsFm35 = Fm35DeliveryValues12Months(fm35LearningDeliveryPeriodisedValues, new[] { "LearnSuppFundCash" }, new[] { "19-24 Traineeship’", "19-24 Traineeship (non-procured)" });

            // ALB
            loansBursaryFunding6Months = AlbDeliveryValues6Months(albLearningDeliveryPeriodisedValues, new[] { "ALBSupportPayment" }, new[] { "Advanced Learner Loans Bursary" });
            loansBursaryFunding12Months = AlbDeliveryValues12Months(albLearningDeliveryPeriodisedValues, new[] { "ALBSupportPayment" }, new[] { "Advanced Learner Loans Bursary" });
            loansAreaCosts6Months = AlbDeliveryValues6Months(albLearningDeliveryPeriodisedValues, new[] { "AreaUpliftBalPayment", "AreaUpliftOnProgPayment" }, new[] { "Advanced Learner Loans Bursary" });
            loansAreaCosts12Months = AlbDeliveryValues12Months(albLearningDeliveryPeriodisedValues, new[] { "AreaUpliftBalPayment", "AreaUpliftOnProgPayment" }, new[] { "Advanced Learner Loans Bursary" });

            // EAS

            adultFundingClaimModel.OtherLearningProgrammeFunding6Months = otherLearningProgramFunding6MonthsFm35 + otherLearningProgramFunding6MonthsEas;
            adultFundingClaimModel.OtherLearningProgrammeFunding12Months = otherLearningProgramFunding12MonthsFm35 + otherLearningProgramFunding12MonthsEas;
            adultFundingClaimModel.OtherLearningLearningSupport6Months = otherLearningSupport6MonthsFm35 + otherLearningSupport6MonthsEas;
            adultFundingClaimModel.OtherLearningLearningSupport12Months = otherLearningSupport12MonthsFm35 + otherLearningSupport12MonthsEas;
            adultFundingClaimModel.Traineeships1924ProgrammeFunding6Months = traineeShips1924ProgrammeFunding6MonthsFm35 + traineeShips1924ProgrammeFunding6MonthsEas;
            adultFundingClaimModel.Traineeships1924ProgrammeFunding12Months = traineeShips1924ProgrammeFunding12MonthsFm35 + traineeShips1924ProgrammeFunding12MonthsEas;
            adultFundingClaimModel.Traineeships1924LearningSupport6Months = traineeShips1924LearningSupport6MonthsFm35 + traineeShips1924LearningSupport6MonthsEas;
            adultFundingClaimModel.Traineeships1924LearningSupport12Months = traineeShips1924LearningSupport12MonthsFm35 + traineeShips1924LearningSupport12MonthsEas;

            adultFundingClaimModel.LoansBursaryFunding6Months = loansBursaryFunding6Months;
            adultFundingClaimModel.LoansBursaryFunding12Months = loansBursaryFunding12Months;
            adultFundingClaimModel.LoansAreaCosts6Months = loansAreaCosts6Months + loansAreasCosts6MonthsEas;
            adultFundingClaimModel.LoansAreaCosts12Months = loansAreaCosts12Months + loansAreasCosts12MonthsEas;
            adultFundingClaimModel.LoansExcessSupport6Months = loansExcessSupport6MonthsEas;
            adultFundingClaimModel.LoansExcessSupport12Months = loansExcessSupport12MonthsEas;

            return adultFundingClaimModel;
        }

        private static decimal AlbDeliveryValues6Months(
            List<ALBLearningDeliveryValues> albLearningDeliveryValues,
            string[] attributes,
            string[] fundLines)
        {
            decimal value = 0;
            var otherLearningProgramFunding = albLearningDeliveryValues.Where(x =>
                attributes.Contains(x.AttributeName) && fundLines.Contains(x.FundLine)).ToList();

            foreach (var deliveryValues in otherLearningProgramFunding)
            {
                value = value +
                    deliveryValues.Period1.GetValueOrDefault() + deliveryValues.Period2.GetValueOrDefault() + deliveryValues.Period3.GetValueOrDefault() +
                    deliveryValues.Period4.GetValueOrDefault() + deliveryValues.Period5.GetValueOrDefault() + deliveryValues.Period6.GetValueOrDefault();
            }

            return value;
        }

        private static decimal AlbDeliveryValues12Months(
            List<ALBLearningDeliveryValues> albLearningDeliveryValues,
            string[] attributes,
            string[] fundLines)
        {
            decimal value = 0;
            var otherLearningProgramFunding = albLearningDeliveryValues.Where(x =>
                attributes.Contains(x.AttributeName) && fundLines.Contains(x.FundLine)).ToList();

            foreach (var deliveryValues in otherLearningProgramFunding)
            {
                value = value +
                     deliveryValues.Period1.GetValueOrDefault() + deliveryValues.Period2.GetValueOrDefault() + deliveryValues.Period3.GetValueOrDefault() +
                    deliveryValues.Period4.GetValueOrDefault() + deliveryValues.Period5.GetValueOrDefault() + deliveryValues.Period6.GetValueOrDefault() +
                    deliveryValues.Period7.GetValueOrDefault() + deliveryValues.Period8.GetValueOrDefault() + deliveryValues.Period9.GetValueOrDefault() +
                    deliveryValues.Period10.GetValueOrDefault() + deliveryValues.Period11.GetValueOrDefault() + deliveryValues.Period12.GetValueOrDefault();
            }

            return value;
        }

        private static decimal Fm35DeliveryValues6Months(
            List<FM35LearningDeliveryValues> fm35LearningDeliveryPeriodisedValues,
            string[] attributes,
            string[] fundLines)
        {
            decimal value = 0;
            var otherLearningProgramFunding = fm35LearningDeliveryPeriodisedValues.Where(x =>
                attributes.Contains(x.AttributeName) && fundLines.Contains(x.FundLine)).ToList();

            foreach (var deliveryValues in otherLearningProgramFunding)
            {
                value = value +
                     deliveryValues.Period1.GetValueOrDefault() + deliveryValues.Period2.GetValueOrDefault() + deliveryValues.Period3.GetValueOrDefault() +
                    deliveryValues.Period4.GetValueOrDefault() + deliveryValues.Period5.GetValueOrDefault() + deliveryValues.Period6.GetValueOrDefault();
            }

            return value;
        }

        private static decimal Fm35DeliveryValues12Months(
            List<FM35LearningDeliveryValues> fm35LearningDeliveryPeriodisedValues,
            string[] attributes,
            string[] fundLines)
        {
            decimal value = 0;
            var otherLearningProgramFunding = fm35LearningDeliveryPeriodisedValues.Where(x =>
                attributes.Contains(x.AttributeName) && fundLines.Contains(x.FundLine)).ToList();

            foreach (var deliveryValues in otherLearningProgramFunding)
            {
                value = value +
                      deliveryValues.Period1.GetValueOrDefault() + deliveryValues.Period2.GetValueOrDefault() + deliveryValues.Period3.GetValueOrDefault() +
                    deliveryValues.Period4.GetValueOrDefault() + deliveryValues.Period5.GetValueOrDefault() + deliveryValues.Period6.GetValueOrDefault() +
                    deliveryValues.Period7.GetValueOrDefault() + deliveryValues.Period8.GetValueOrDefault() + deliveryValues.Period9.GetValueOrDefault() +
                    deliveryValues.Period10.GetValueOrDefault() + deliveryValues.Period11.GetValueOrDefault() + deliveryValues.Period12.GetValueOrDefault();
            }

            return value;
        }
    }
}