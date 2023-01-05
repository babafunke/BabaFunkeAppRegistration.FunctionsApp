using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Threading.Tasks;

namespace BabaFunkeAppRegistration.Orchestrators
{
    public class RegisterAppUserOrchestrator
    {
        [FunctionName(nameof(RegisterAppUser))]
        public async Task<object> RegisterAppUser([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            logger = context.CreateReplaySafeLogger(logger);

            var appUser = context.GetInput<AppUser>();
            appUser.OrchestrationId = context.InstanceId;

            ActivityResponse validateDataResponse = null;
            ActivityResponse persistDataResponse = null;
            ActivityResponse sendConfirmEmailResponse = null;
            bool registrationCompleteStatus = false;

            try
            {
                logger.LogInformation("About to validate the app user payload");
                validateDataResponse = await context.CallActivityAsync<ActivityResponse>(StringConstants.ValidateAppUserActivity, appUser);

                if (!validateDataResponse.IsSuccess)
                {
                    throw new Exception();
                }

                logger.LogInformation("About to persist data to Table storage");
                persistDataResponse = await context.CallActivityAsync<ActivityResponse>(StringConstants.PersistAppUserActivity, appUser);

                if (!persistDataResponse.IsSuccess)
                {
                    throw new Exception();
                }

                logger.LogInformation("About to send confirmation email");
                appUser.ReferenceCode = persistDataResponse.Message;
                sendConfirmEmailResponse = await context.CallActivityAsync<ActivityResponse>(StringConstants.SendConfirmEmailActivity, appUser);

                if (!sendConfirmEmailResponse.IsSuccess)
                {
                    throw new Exception();
                }

                logger.LogInformation("About to complete registration");
                registrationCompleteStatus = await context.CallActivityAsync<bool>(StringConstants.CompleteRegistrationActivity, appUser);
            }
            catch (Exception)
            {
                return new
                {
                    Summary = "Failed",
                    ValidateDataResponse = validateDataResponse,
                    PersistDataResponse = persistDataResponse,
                    SendConfirmEmailResponse = sendConfirmEmailResponse,
                    RegistrationCompleteStatus = registrationCompleteStatus
                };
            }

            return new
            {
                Summary = "Successful",
                ValidateDataResponse = validateDataResponse,
                PersistDataResponse = persistDataResponse,
                SendConfirmEmailResponse = sendConfirmEmailResponse,
                RegistrationCompleteStatus = registrationCompleteStatus
            };
        }
    }
}