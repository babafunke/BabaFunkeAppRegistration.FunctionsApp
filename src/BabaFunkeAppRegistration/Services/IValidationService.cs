using Shared;

namespace BabaFunkeAppRegistration.Services
{
    public interface IValidationService
    {
        ActivityResponse IsValidated(AppUser appUser);
    }
}