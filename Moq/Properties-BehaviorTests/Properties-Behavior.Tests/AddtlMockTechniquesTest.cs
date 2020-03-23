using System;
using Xunit;
using Moq;

namespace CreditCardApplications.Tests
{
    public class AddtlMockTechniquesTest
    {
        [Fact]
        public void ReferWhenFrequentFlyerValidationError()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            // 06-01 Throw Exception
            // specify what kind of exception based on scenario
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws(new Exception("Custom Message"));


            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void IncrementLookupCount()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>()))
                .Returns(true)
                .Raises(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);


            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { FrequentFlyerNumber = "x", Age = 25 };

            sut.Evaluate(application);

            //06 - 02 Events
            //mockValidator.Raise(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);

            Assert.Equal(1, sut.ValidatorLookupCount);
        }
    }
}
