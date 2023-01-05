using Shared;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace BabaFunkeEmailConfirmation.Endpoints
{
    public class ConfirmAndSendWelcomeEmail
    {
        [FunctionName(nameof(ConfirmAndSendWelcomeEmail))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "confirmandwelcomeemail/{code}/{email}")] HttpRequest req,
            [SendGrid(ApiKey = "SendGridApiKey", From = "%SenderEmail%")] IAsyncCollector<SendGridMessage> messageCollector,
            [Table("BabaFunkeAppUsers", "AzureWebJobsStorage")] TableClient tableClient,
            string code,
            string email,
            ILogger logger)
        {
            logger.LogInformation("Confirming query string for user");

            var response = req.GetQueryParameterDictionary()["confirmstatus"];
            if (string.IsNullOrEmpty(response))
            {
                return new BadRequestObjectResult("The confirmstatus value is missing!");
            }

            logger.LogInformation("Checking if record exists in the table");

            var record = await tableClient.GetEntityAsync<AppUserEntity>(StringConstants.AppUserPartitionKey, email);
            if (record == null)
            {
                return new NotFoundObjectResult("Record not found!");
            }

            logger.LogInformation("Checking if code exists for the record");

            if (record.Value.ReferenceCode != code)
            {
                return new NotFoundObjectResult("The code is mismatched!");
            }

            logger.LogInformation($"Updating record for {record.Value.RowKey}");

            var appUser = new AppUserEntity
            {
                PartitionKey = record.Value.PartitionKey,
                RowKey = record.Value.RowKey,
                Username = record.Value.Username,
                EmailConfirmed = bool.Parse(response),
                OrchestrationId = record.Value.OrchestrationId,
                ReferenceCode = record.Value.ReferenceCode,
                Created = record.Value.Created,
                Updated = DateTime.UtcNow
            };

            var updateResponse = await tableClient.UpdateEntityAsync(appUser, ETag.All,TableUpdateMode.Merge);
            if (updateResponse.IsError)
            {
                return new BadRequestObjectResult("Problem updating record!");
            }

            logger.LogInformation($"Sending welcome email to {appUser.RowKey}");

            var subject = "Welcome to BabaFunke App.";
            var body = $"Welcome to the Baba Funke App.<br>You have now completed your registration.<br>";
            var message = new SendGridMessage
            {
                Subject = subject
            };
            message.AddContent("text/html", body);
            message.AddTo(appUser.RowKey);

            await messageCollector.AddAsync(message);
            await messageCollector.FlushAsync();

            return new OkObjectResult($"Confirmation complete!");
        }
    }
}