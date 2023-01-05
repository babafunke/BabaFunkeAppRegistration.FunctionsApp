using BabaFunkeAppRegistration.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Shared;
using System;
using System.Threading.Tasks;

namespace BabaFunkeAppRegistration.Activities
{
    public class ValidateAppUserActivity
    {
        private readonly IValidationService _validationService;

        public ValidateAppUserActivity(IValidationService validationService)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }

        [FunctionName(StringConstants.ValidateAppUserActivity)]
        public async Task<ActivityResponse> RunAsync([ActivityTrigger] AppUser appUser)
        {
            return  await  Task.Run(() => _validationService.IsValidated(appUser));
        }
    }
}