using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Threading.Tasks;

namespace BabaFunkeAppRegistration.Activities
{
    public class PersistAppUserActivity
    {
        [FunctionName(StringConstants.PersistAppUserActivity)]
        public async Task<ActivityResponse> RunAsync([ActivityTrigger] AppUser appUser,
            [Table("BabaFunkeAppUsers", "AzureWebJobsStorage")] TableClient tableClient,
            ILogger logger)
        {
            try
            {
                var referenceCode = Guid.NewGuid().ToString("N");

                logger.LogInformation($"Adding app user to table storage");

                var appUserEntity = new AppUserEntity
                {
                    RowKey = appUser.Email,
                    Username = appUser.Username,
                    EmailConfirmed = false,
                    OrchestrationId = appUser.OrchestrationId,
                    ReferenceCode = referenceCode,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                var tableResponse = await tableClient.AddEntityAsync(appUserEntity);
                if (tableResponse.IsError)
                {
                    return new ActivityResponse(StringConstants.PersistAppUserActivity, $"Problem saving to table. Reason: {tableResponse.ReasonPhrase}");
                }

                return new ActivityResponse
                {
                    ActivityName = StringConstants.PersistAppUserActivity,
                    IsSuccess = true,
                    Message = referenceCode,
                    ActivityStatus = ActivityStatus.Succeeded
                };
            }
            catch (Exception ex)
            {
                return new ActivityResponse(StringConstants.PersistAppUserActivity, $"Exception thrown. {ex.Message}");
            }
        }
    }
}