using Azure;
using Azure.Data.Tables;
using BabaFunkeEmailConfirmation.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using SendGrid.Helpers.Mail;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BabaFunkeAppRegistration.Tests.BabaFunkeEmailConfirmation
{
    public class ConfirmAndSendWelcomeEmailFacts
    {
        private readonly ConfirmAndSendWelcomeEmail _sut;
        private readonly Mock<HttpRequest> _httpRequest;
        private readonly Mock<IAsyncCollector<SendGridMessage>> _messageCollector;
        private readonly Mock<TableClient> _tableClient;
        private readonly Mock<ILogger> _logger;
        private readonly string _code = "123456gghgjg";
        private readonly string _email = "example@example.com";

        public ConfirmAndSendWelcomeEmailFacts()
        {
            _sut = new ConfirmAndSendWelcomeEmail();
            _httpRequest = new Mock<HttpRequest>();
            _messageCollector = new Mock<IAsyncCollector<SendGridMessage>>();
            _tableClient = new Mock<TableClient>();
            _logger = new Mock<ILogger>();
        }

        [Fact]
        public async Task Run_ShouldReturnBadRequestObjectResult_IfQueryStringIsEmpty()
        {
            var mockDict = new Dictionary<string, StringValues> { { "confirmstatus", string.Empty } };

            _httpRequest.Setup(h => h.Query).Returns(new QueryCollection(mockDict));

            var result = await _sut.Run(_httpRequest.Object, _messageCollector.Object, _tableClient.Object, _code, _email, _logger.Object);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Run_ShouldNotFoundObjectResult_IfTheRecordDoesNotExist()
        {
            var mockDict = new Dictionary<string, StringValues> { { "confirmstatus", "Confirmed" } };
            var mockResponse = new Mock<Response<AppUserEntity>>();

            _httpRequest.Setup(h => h.Query).Returns(new QueryCollection(mockDict));
            mockResponse.Setup(m => m.Value).Returns((AppUserEntity)null);
            _tableClient.Setup(t => t.GetEntityAsync<AppUserEntity>(StringConstants.AppUserPartitionKey, "def@def.com", null, default)).ReturnsAsync(mockResponse.Object);

            var result = await _sut.Run(_httpRequest.Object, _messageCollector.Object, _tableClient.Object, _code, _email, _logger.Object);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Run_ShouldReturnNotFoundObjectResult_IfTheReferenceCodeDoesNotExist()
        {
            var mockDict = new Dictionary<string, StringValues> { { "confirmstatus", "Confirmed" } };
            var mockResponse = new Mock<Response<AppUserEntity>>();

            _httpRequest.Setup(h => h.Query).Returns(new QueryCollection(mockDict));
            mockResponse.Setup(m => m.Value).Returns(GetMockAppUserEntity());
            _tableClient.Setup(t => t.GetEntityAsync<AppUserEntity>(StringConstants.AppUserPartitionKey, _email, null, default)).ReturnsAsync(mockResponse.Object);

            var result = await _sut.Run(_httpRequest.Object, _messageCollector.Object, _tableClient.Object, "fakecode", _email, _logger.Object);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Run_ShouldReturnBadRequestObjectResult_IfTheTableRecordIsNotUpdated()
        {
            var mockDict = new Dictionary<string, StringValues> { { "confirmstatus", "true" } };
            var mockResponse = new Mock<Response<AppUserEntity>>();
            var mockUpdateResponse = new Mock<Response>();

            _httpRequest.Setup(h => h.Query).Returns(new QueryCollection(mockDict));
            mockResponse.Setup(m => m.Value).Returns(GetMockAppUserEntity());
            mockUpdateResponse.Setup(m => m.IsError).Returns(true);
            _tableClient.Setup(t => t.GetEntityAsync<AppUserEntity>(StringConstants.AppUserPartitionKey, _email, null, default)).ReturnsAsync(mockResponse.Object);
            _tableClient.Setup(t => t.UpdateEntityAsync(It.IsAny<AppUserEntity>(), ETag.All, TableUpdateMode.Merge, default)).ReturnsAsync(mockUpdateResponse.Object);

            var result = await _sut.Run(_httpRequest.Object, _messageCollector.Object, _tableClient.Object, _code, _email, _logger.Object);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Run_ShouldCallUpdateEntity_IfTheRecordExists()
        {
            var mockDict = new Dictionary<string, StringValues> { { "confirmstatus", "true" } };
            var mockResponse = new Mock<Response<AppUserEntity>>();
            var mockUpdateResponse = new Mock<Response>();

            _httpRequest.Setup(h => h.Query).Returns(new QueryCollection(mockDict));
            mockResponse.Setup(m => m.Value).Returns(GetMockAppUserEntity());
            mockUpdateResponse.Setup(m => m.IsError).Returns(false);
            _tableClient.Setup(t => t.GetEntityAsync<AppUserEntity>(StringConstants.AppUserPartitionKey, _email, null, default)).ReturnsAsync(mockResponse.Object);
            _tableClient.Setup(t => t.UpdateEntityAsync(It.IsAny<AppUserEntity>(), ETag.All, TableUpdateMode.Merge, default)).ReturnsAsync(mockUpdateResponse.Object);

            var result = await _sut.Run(_httpRequest.Object, _messageCollector.Object, _tableClient.Object, _code, _email, _logger.Object);

            _tableClient.Verify(t => t.UpdateEntityAsync(It.IsAny<AppUserEntity>(), ETag.All, TableUpdateMode.Merge, default), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Run_ShouldReturnOkObjectResult_IfTheOperationCompletes()
        {
            var mockDict = new Dictionary<string, StringValues> { { "confirmstatus", "true" } };
            var mockResponse = new Mock<Response<AppUserEntity>>();
            var mockUpdateResponse = new Mock<Response>();

            _httpRequest.Setup(h => h.Query).Returns(new QueryCollection(mockDict));
            mockResponse.Setup(m => m.Value).Returns(GetMockAppUserEntity());
            mockUpdateResponse.Setup(m => m.IsError).Returns(false);
            _tableClient.Setup(t => t.GetEntityAsync<AppUserEntity>(StringConstants.AppUserPartitionKey, _email, null, default)).ReturnsAsync(mockResponse.Object);
            _tableClient.Setup(t => t.UpdateEntityAsync(It.IsAny<AppUserEntity>(), ETag.All, TableUpdateMode.Merge, default)).ReturnsAsync(mockUpdateResponse.Object);

            var result = await _sut.Run(_httpRequest.Object, _messageCollector.Object, _tableClient.Object, _code, _email, _logger.Object);

            Assert.IsType<OkObjectResult>(result);
        }

        private AppUserEntity GetMockAppUserEntity()
        {
            return new AppUserEntity
            {
                PartitionKey = StringConstants.AppUserPartitionKey,
                RowKey = _email,
                Username = "BabaFunke",
                EmailConfirmed = false,
                OrchestrationId = _code,
                ReferenceCode = _code,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
        }
    }
}