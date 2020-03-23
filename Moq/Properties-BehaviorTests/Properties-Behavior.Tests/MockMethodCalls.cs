using System;
using Xunit;
using Moq;

namespace CreditCardApplications.Tests
{
    public class MockMethodCalls
    {
        //01 - Creating First Mock
        [Fact]
        public void AcceptHighIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            //var sut = new CreditCardApplicationEvaluator(null);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {

            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //05 Refactoring Tests
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            // 04-04 Default Value Behavior for Loose Mocks
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey);
            mockValidator.DefaultValue = DefaultValue.Mock;


            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        // other evaluator test conditions
        // 02 - Method Return Values
        [Fact]
        public void DeclineLowIncomeApplications()
        {

            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //setup values on mock object
            //mockValidator.Setup(x => x.IsValid("x")).Returns(true);

            //03 Argument Matching
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith('x'))))
            //    .Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.IsIn("x", "y", "z")))
            //    .Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.IsInRange("b", "z", Range.Inclusive)))
            //    .Returns(true);
            mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]",
                          System.Text.RegularExpressions.RegexOptions.None)))
                .Returns(true);


            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            

            var application = new CreditCardApplication
            { 
                GrossAnnualIncome = 19_999,
                Age =  42,
                FrequentFlyerNumber = "a"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        // 04 Strict Mock
        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {

            //Mock<IFrequentFlyerNumberValidator> mockValidator = 
            //    new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Strict);
            Mock<IFrequentFlyerNumberValidator> mockValidator = 
                new Mock<IFrequentFlyerNumberValidator>(); // removed strict for method properties

            // we have to explicitly setup each called 
            // methods for Strict Mock Behavior
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        //06 Out Parameters
        [Fact]
        public void DeclineLowIncomeApplicationsOutDemo()
        {

            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            bool isValid = true; // false for mock properties
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);


            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "a"
            };

            CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }
    }
}
