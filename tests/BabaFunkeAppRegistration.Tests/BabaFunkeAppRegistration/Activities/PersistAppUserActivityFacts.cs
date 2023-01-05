using Azure;
using Azure.Data.Tables;
using BabaFunkeAppRegistration.Activities;
using BabaFunkeAppRegistration.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace BabaFunkeAppRegistration.Tests.BabaFunkeAppRegistration.Activities
{
    public class PersistAppUserActivityFacts
    {
        private readonly PersistAppUserActivity _sut;
        private readonly Mock<TableClient> _tableClient;
        private readonly Mock<ILogger> _logger;
        private AppUser _appUser;

        public PersistAppUserActivityFacts()
        {
            _tableClient = new Mock<TableClient>();
            _logger = new Mock<ILogger>();
            _appUser = TestHelpers.GetAppuser();
            _sut = new PersistAppUserActivity();
        }

        [Fact]
        public async Task RunAsync_ShouldReturnFalseResponse_IfPersistenceFails()
        {
            var mockResponse = new Mock<Response>();

            mockResponse.Setup(m => m.IsError).Returns(true);
            _tableClient.Setup(t => t.AddEntityAsync(It.IsAny<AppUserEntity>(), default)).ReturnsAsync(mockResponse.Object);

            var result = await _sut.RunAsync(_appUser, _tableClient.Object, _logger.Object);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RunAsync_ShouldReturnTrueResponse_IfPersistenceSucceeds()
        {
            var mockResponse = new Mock<Response>();

            mockResponse.Setup(m => m.IsError).Returns(false);
            _tableClient.Setup(t => t.AddEntityAsync(It.IsAny<AppUserEntity>(), default)).ReturnsAsync(mockResponse.Object);

            var result = await _sut.RunAsync(_appUser, _tableClient.Object, _logger.Object);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task RunAsync_ShouldReturnActivityResponse()
        {
            var mockResponse = new Mock<Response>();

            mockResponse.Setup(m => m.IsError).Returns(false);
            _tableClient.Setup(t => t.AddEntityAsync(It.IsAny<AppUserEntity>(), default)).ReturnsAsync(mockResponse.Object);

            var result = await _sut.RunAsync(_appUser, _tableClient.Object, _logger.Object);

            Assert.IsType<ActivityResponse>(result);
        }
    }
}