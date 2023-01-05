using Shared;
using System.Text.RegularExpressions;

namespace BabaFunkeAppRegistration.Services
{
    public class ValidationService: IValidationService
    {
        public ActivityResponse IsValidated(AppUser appUser)
        {
            if (string.IsNullOrEmpty(appUser.Username))
            {
                var x = new ActivityResponse(StringConstants.ValidateAppUserActivity, "The username is missing.");
                return x;
            }

            if (string.IsNullOrEmpty(appUser.Email))
            {
                return new ActivityResponse(StringConstants.ValidateAppUserActivity, "The Email is missing.");
            }

            var regex = new Regex(@"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            if (!regex.IsMatch(appUser.Email))
            {
                return new ActivityResponse(StringConstants.ValidateAppUserActivity, "The Email address is invalid.");
            }

            return new ActivityResponse
            {
                ActivityName = StringConstants.ValidateAppUserActivity,
                IsSuccess = true,
                Message = "Successfully validated AppUser!",
                ActivityStatus = ActivityStatus.Succeeded
            };
        }
    }
}