using System;
using Xunit;
using Moq;

namespace Moq_XUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

        }

        [Fact]
        public void Can_Evade_Trouble()
        {
            // Arrange (set up a scenario)
            Mock<IVehicle> mock = new Mock<IVehicle>();
            mock.Setup(x => x.ApplyBrakes()).Returns(true);
            Driver target = new Driver(mock.Object);

            // Act (attempt the operation)
            bool success = target.EvasiveManeuvers(false);

            // Assert (verify the result)
            Assert.True(success);
            mock.Verify(x => x.HonkHorn(), Times.Never());
            mock.Verify(x => x.ApplyBrakes(), Times.Once());
        }

        [Fact]
        public void Can_Evade_Trouble_And_Alert_Offending_Driver()
        {
            // Arrange (set up a scenario)
            Mock<IVehicle> mock = new Mock<IVehicle>();
            mock.Setup(x => x.HonkHorn()).Returns(true);
            mock.Setup(x => x.ApplyBrakes()).Returns(true);
            Driver target = new Driver(mock.Object);

            // Act (attempt the operation)
            bool success = target.EvasiveManeuvers(true);

            // Assert (verify the result)
            Assert.True(success);
            mock.Verify(x => x.HonkHorn(), Times.Once());
            mock.Verify(x => x.ApplyBrakes(), Times.Once());
        }
    }
}
