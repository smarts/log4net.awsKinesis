using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace log4net.Ext.ErrorHandler.UnitTests
{
    [TestClass]
    public class ErrorTripSwitchTests
    {
        [TestMethod]
        public void ErrorTripSwitchTriggersAndThenResets()
        {
            var target = new CircuitBreakerErrorHandler { TripErrorCountPerMinute = 10 };

            var mockMinuteRetriever = new Mock<IDateTimeMinuteRetriever>();
            mockMinuteRetriever.Setup(x => x.GetMinute()).Returns(1);
            target.MinuteRetriever = mockMinuteRetriever.Object;

            for (var i = 0; i < 10; i++)
            {
                // Up to 10 errors, trip switch is not tripped
                Assert.IsFalse(target.IsTripped);
                target.Error(null);
            }

            // After 10 errors, it is tripped
            Assert.IsTrue(target.IsTripped);

            mockMinuteRetriever.Setup(x => x.GetMinute()).Returns(2);

            // The following minute, it has reset
            Assert.IsFalse(target.IsTripped);
        }

        [TestMethod]
        public void ErrorTripSwitchRemembersTripFromPreviousMinute()
        {
            var target = new CircuitBreakerErrorHandler { TripErrorCountPerMinute = 10 };

            var mockMinuteRetriever = new Mock<IDateTimeMinuteRetriever>();
            mockMinuteRetriever.Setup(x => x.GetMinute()).Returns(1);
            target.MinuteRetriever = mockMinuteRetriever.Object;

            // Trip minute
            for (var i = 0; i < 10; i++)
            {
                target.Error(null);
            }

            // New minute. Switch not tripped.
            mockMinuteRetriever.Setup(x => x.GetMinute()).Returns(2);
            Assert.IsFalse(target.IsTripped);

            // Previous minute is still tripped.
            mockMinuteRetriever.Setup(x => x.GetMinute()).Returns(1);
            Assert.IsTrue(target.IsTripped);
        }

        [TestMethod]
        public void ErrorTripSwitchDoesNotRememberTwoMinuteOldTrip()
        {
            var target = new CircuitBreakerErrorHandler() { TripErrorCountPerMinute = 10 };

            var mockMinuteRetriever = new Mock<IDateTimeMinuteRetriever>();
            mockMinuteRetriever.Setup(x => x.GetMinute()).Returns(1);
            target.MinuteRetriever = mockMinuteRetriever.Object;

            // Trip minute
            for (var i = 0; i < 10; i++)
            {
                target.Error(null);
            }

            // New minute. Switch not tripped.
            mockMinuteRetriever.Setup(x => x.GetMinute()).Returns(3);
            Assert.IsFalse(target.IsTripped);

            // Switch not tripped for time from two minutes ago.
            mockMinuteRetriever.Setup(x => x.GetMinute()).Returns(1);
            Assert.IsFalse(target.IsTripped);
        }
    }
}
