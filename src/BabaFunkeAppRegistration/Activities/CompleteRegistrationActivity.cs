using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Shared;
using System.Threading.Tasks;

namespace BabaFunkeAppRegistration.Activities
{
    public class CompleteRegistrationActivity
    {
        [FunctionName(StringConstants.CompleteRegistrationActivity)]
        public async Task<bool> RunAsync([ActivityTrigger] AppUser appUser, ILogger logger)
        {
            logger.LogInformation($"Registration completed for {appUser.Email}");
            return await Task.Run(() => true);
        }
    }
}