using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using Shared;
using System;
using System.Threading.Tasks;

namespace BabaFunkeAppRegistration.Activities
{
    public class SendConfirmEmailActivity
    {
        [FunctionName(StringConstants.SendConfirmEmailActivity)]
        public async Task<ActivityResponse> RunAsync([ActivityTrigger] AppUser appUser,
            [SendGrid(ApiKey = "SendGridApiKey")] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Sending email confirmation request to {appUser.Email}");

                var senderEmail = new EmailAddress(Environment.GetEnvironmentVariable("SenderEmail"));
                var host = Environment.GetEnvironmentVariable("FuntionAppEmailHost");
                var functionAppAddress = $"{host}/api/confirmandwelcomeemail/{appUser.ReferenceCode}/{appUser.Email}";
                var confirmationLink = $"{functionAppAddress}?confirmstatus=true";
                var body = $"Please click the link to confirm your email and complete your registration.<br>" +
                    $"<a href=\"{confirmationLink}\">Approve</a><br>";

                var message = new SendGridMessage();
                message.AddTo(appUser.Email);
                message.AddContent("text/html", body);
                message.SetFrom(senderEmail);
                message.SetSubject("Registration Confirmation for BabaFunke App.");

                await messageCollector.AddAsync(message);

                return new ActivityResponse
                {
                    ActivityName = StringConstants.SendConfirmEmailActivity,
                    IsSuccess = true,
                    Message = "Successfully sent email confirmation!",
                    ActivityStatus = ActivityStatus.Succeeded
                };
            }
            catch (Exception ex)
            {
                return new ActivityResponse(StringConstants.SendConfirmEmailActivity, $"Exception thrown. {ex.Message}");
            }
        }
    }
}