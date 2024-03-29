﻿using System;

namespace CreditCardApplications
{
    public class CreditCardApplicationEvaluator
    {
        private readonly IFrequentFlyerNumberValidator _validator;

        private const int AutoReferralMaxAge = 20;
        private const int HighIncomeThreshhold = 100_000;
        private const int LowIncomeThreshhold = 20_000;

        public int ValidatorLookupCount { get; private set; }
        public CreditCardApplicationEvaluator(IFrequentFlyerNumberValidator validator)
        {
            _validator = validator ??
                throw new System.ArgumentNullException(nameof(validator));

            _validator.ValidatorLookupPerformed += ValidatorLookupPerformed;
        }

        private void ValidatorLookupPerformed(object sender, EventArgs e)
        {
            ValidatorLookupCount++;
        }

        public CreditCardApplicationDecision Evaluate(CreditCardApplication application)
        {
            if (application.GrossAnnualIncome >= HighIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoAccepted;
            }

            //if (_validator.LicenseKey == "EXPIRED")
            //    return CreditCardApplicationDecision.ReferredToHuman;
            if (_validator.ServiceInformation.License.LicenseKey == "EXPIRED")
                return CreditCardApplicationDecision.ReferredToHuman;

            _validator.ValidationMode = application.Age >= 30 ? ValidationMode.Detailed : 
                                                                ValidationMode.Quick;

            var isValidFrequentFlyerNumber =
                _validator.IsValid(application.FrequentFlyerNumber);

            bool isValidFrequentFlyerNumberExc;
            try
            {
                isValidFrequentFlyerNumberExc =
                    _validator.IsValid(application.FrequentFlyerNumber);
            }
            catch (System.Exception)
            {

                return CreditCardApplicationDecision.ReferredToHuman;
            }
            //05-04 Method Times Implement
            //_validator.IsValid(application.FrequentFlyerNumber);
            if (!isValidFrequentFlyerNumber)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.Age <= AutoReferralMaxAge)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.GrossAnnualIncome < LowIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoDeclined;
            }

            return CreditCardApplicationDecision.ReferredToHuman;
        }

        //06 Out Parameters
        public CreditCardApplicationDecision EvaluateUsingOut(CreditCardApplication application)
        {
            if (application.GrossAnnualIncome >= HighIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoAccepted;
            }

            _validator.IsValid(application.FrequentFlyerNumber, out var isValidFrequentFlyerNumber);

            if (!isValidFrequentFlyerNumber)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.Age <= AutoReferralMaxAge)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.GrossAnnualIncome < LowIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoDeclined;
            }

            return CreditCardApplicationDecision.ReferredToHuman;
        }
    }
}
