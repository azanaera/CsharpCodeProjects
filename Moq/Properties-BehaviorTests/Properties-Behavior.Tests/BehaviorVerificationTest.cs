using System;
using Xunit;
using Moq;

namespace CreditCardApplications.Tests
{
    public class BehaviorVerificationTest
    {
        // 05-01 Verify Method
        [Fact]
        public void ShouldValidateFrequentlyFlyerNumberForLowIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            // 05-01 Verify Method
            var application = new CreditCardApplication();
            //var application = new CreditCardApplication { FrequentFlyerNumber = "q" };

            sut.Evaluate(application);

            // 05-03 Method Not Called
            mockValidator.Verify(x => x.IsValid(It.IsNotNull<string>()), "Frequent flyer passed should not be null");
        }

        // 05-04  Method Times
        [Fact]
        public void ValidateFrequentlyFlyerNumberForLowIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { FrequentFlyerNumber = "q" };

            sut.Evaluate(application);
            // Shorthand - Times.Once // Or Times.Exactly(1)
            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Exactly(1));
        }


        // 05-02 Method Not Called
        [Fact]
        public void NotValidateFrequentFlyerNumberForHighIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            sut.Evaluate(application);
            // 05-02 Times.Never verifies if the method was  not called
            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }

        // 05-05 Getter
        [Fact]
        public void CheckLicenseKeyForLowIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 99_000 }; //99 for pass, 100 for fail

            sut.Evaluate(application);

            mockValidator.Verify(x => x.ServiceInformation.License.LicenseKey, Times.Once);
        }

        // 05-06 Setter
        [Fact]
        public void SetDetailedLookupForOlderApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);
            //05-06 verify directly for setters, much easier
            //mockValidator.VerifySet(x => x.ValidationMode = ValidationMode.Detailed);
            mockValidator.VerifySet(x => x.ValidationMode = It.IsAny<ValidationMode>(), Times.Once);
        }
    }
}
