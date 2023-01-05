using BabaFunkeAppRegistration.Activities;
using Microsoft.Extensions.Logging;
using Moq;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace BabaFunkeAppRegistration.Tests.BabaFunkeAppRegistration.Activities
{
    public class CompleteRegistrationActivityFacts
    {
        private readonly CompleteRegistrationActivity _sut;
        private readonly Mock<ILogger> _logger;

        public CompleteRegistrationActivityFacts()
        {
            _sut = new CompleteRegistrationActivity();
            _logger = new Mock<ILogger>();
        }

        [Fact]
        public async Task RunAsync_ShouldCallActivityResponse()
        {

            var result = await _sut.RunAsync(new AppUser(), _logger.Object);

            Assert.IsType<bool>(result);
        }
    }
}