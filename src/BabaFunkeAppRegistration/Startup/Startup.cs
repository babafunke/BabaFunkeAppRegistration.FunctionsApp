using BabaFunkeAppRegistration.Services;
using BabaFunkeAppRegistration.Startup;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace BabaFunkeAppRegistration.Startup
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IValidationService, ValidationService>();
        }
    }
}