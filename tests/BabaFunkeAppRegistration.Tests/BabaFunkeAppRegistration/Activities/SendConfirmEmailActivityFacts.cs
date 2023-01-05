using BabaFunkeAppRegistration.Activities;
using BabaFunkeAppRegistration.Tests.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using SendGrid.Helpers.Mail;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace BabaFunkeAppRegistration.Tests.BabaFunkeAppRegistration.Activities
{
    public class SendConfirmationEmailActivityFacts
    {
        private readonly SendConfirmEmailActivity _sut;
        private readonly Mock<IAsyncCollector<SendGridMessage>> _messageCollector;
        private readonly Mock<ILogger> _logger;
        private AppUser _appUser;

        public SendConfirmationEmailActivityFacts()
        {
            _messageCollector = new Mock<IAsyncCollector<SendGridMessage>>();
            _logger = new Mock<ILogger>();
            _appUser = TestHelpers.GetAppuser();
            _sut = new SendConfirmEmailActivity();
        }

        [Fact]
        public async Task RunAsync_ShouldCallAddAsync()
        {
            var result = await _sut.RunAsync(_appUser, _messageCollector.Object, _logger.Object);

            _messageCollector.Verify(m => m.AddAsync(It.IsAny<SendGridMessage>(), default), Times.Once());
        }

        [Fact]
        public async Task RunAsync_ShouldReturnActivityResponse()
        {

            _messageCollector.Setup(m => m.AddAsync(It.IsAny<SendGridMessage>(), default));

            var result = await _sut.RunAsync(_appUser, _messageCollector.Object, _logger.Object);

            Assert.IsType<ActivityResponse>(result);
        }

    }
}