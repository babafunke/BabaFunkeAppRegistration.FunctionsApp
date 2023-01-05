using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BabaFunkeAppRegistration.Starters
{
    public class RegisterAppUserStarter
    {
        [FunctionName(nameof(HttpStart))]
        public async Task<IActionResult> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Processing the HttpRequest body");

                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult("The body is missing!");
                }

                var appUser = JsonConvert.DeserializeObject<AppUser>(requestBody);

                if (appUser == null)
                {
                    return new BadRequestObjectResult("Invalid payload! Cannot be converted to the model.");
                }

                // Call the Orchestrator passing in the appUser as input
                var instanceId = await starter.StartNewAsync(StringConstants.RegisterAppUserOrchestrator, null, appUser);

                // Wait until the process is complete or timeout after 30 seconds so we can notify the caller (client)
                var response = await starter.WaitForCompletionOrCreateCheckStatusResponseAsync(req, instanceId, TimeSpan.FromSeconds(30));

                // Get the status of the process
                var status = await starter.GetStatusAsync(instanceId);

                // If it timed out
                if (status.RuntimeStatus != OrchestrationRuntimeStatus.Completed)
                {
                    await starter.TerminateAsync(instanceId, "Timeout! Process took longer than expected!");

                    return new ContentResult()
                    {
                        Content = "{ Error: \"Timeout! Process took longer than expected!\"\nPlease try again! }",
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Exception thrown: {ex.Message}");
                return new BadRequestObjectResult("Something went wrong! Please try again!");
            }
        }
    }
}