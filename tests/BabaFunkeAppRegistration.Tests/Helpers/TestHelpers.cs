using Shared;
using System;

namespace BabaFunkeAppRegistration.Tests.Helpers
{
    public static class TestHelpers
    {
        public static AppUser GetAppuser()
        {
            return new AppUser
            {
                Username = "Baba Funke",
                Email = "babafunke@babafunke.com",
                EmailConfirmed = false,
                OrchestrationId = "afsgg5676gg",
                ReferenceCode = "afsgg5676gg",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
        }
    }
}