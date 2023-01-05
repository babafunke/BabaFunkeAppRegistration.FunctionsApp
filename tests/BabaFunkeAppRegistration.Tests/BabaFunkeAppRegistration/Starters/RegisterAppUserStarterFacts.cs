using BabaFunkeAppRegistration.Starters;
using BabaFunkeAppRegistration.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using Shared;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace BabaFunkeAppRegistration.Tests.BabaFunkeAppRegistration.Starters
{
    public class RegisterAppUserStarterFacts
    {
        private readonly RegisterAppUserStarter _sut;
        private readonly Mock<HttpRequest> _httpRequest;
        private readonly Mock<IDurableOrchestrationClient> _orchestrationClient;
        private readonly Mock<ILogger> _logger;
        private readonly AppUser _appUser;

        public RegisterAppUserStarterFacts()
        {
            _sut = new RegisterAppUserStarter();
            _httpRequest = new Mock<HttpRequest>();
            _orchestrationClient = new Mock<IDurableOrchestrationClient>();
            _logger = new Mock<ILogger>();
            _appUser = TestHelpers.GetAppuser();
        }

        [Fact]
        public async Task HttpStart_ShouldReturnBadRequestObjectResult_IfBodyIsMissing()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));

            _httpRequest.Setup(r => r.Body).Returns(stream);

            var result = await _sut.HttpStart(_httpRequest.Object, _orchestrationClient.Object, _logger.Object);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task HttpStart_ShouldReturnBadRequestObjectResult_IfBodyIsUnserializableToAppUser()
        {
            dynamic body = string.Empty;
            string json = JsonSerializer.Serialize(body);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            _httpRequest.Setup(r => r.Body).Returns(stream);

            var result = await _sut.HttpStart(_httpRequest.Object, _orchestrationClient.Object, _logger.Object);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task HttpStart_ShouldInvokeTheOrchestrator()
        {
            string json = JsonSerializer.Serialize(_appUser);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            _httpRequest.Setup(r => r.Body).Returns(stream);

            await _sut.HttpStart(_httpRequest.Object, _orchestrationClient.Object, _logger.Object);

            _orchestrationClient.Verify(d => d.StartNewAsync(StringConstants.RegisterAppUserOrchestrator, null, It.IsAny<AppUser>()), Times.Once);
        }
    }
}