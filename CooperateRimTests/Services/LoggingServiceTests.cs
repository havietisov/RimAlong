using CooperateRim.Services;
using NUnit.Framework;

namespace CooperateRimTests.Services
{
    [TestFixture]
    class LoggingServiceTests
    {
        private LoggingService logger;

        [SetUp]
        public void Setup()
        {
            logger = new LoggingService(null);
        }

        [Test]
        public void ShouldNotWriteMessageIfNoLogger()
        {
            logger.Message("");
        }

        [Test]
        public void ShouldNotWriteErrorIfNoLogger()
        {
            logger.Error("");
        }
    }
}
