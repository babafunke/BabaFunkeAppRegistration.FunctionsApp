using BabaFunkeAppRegistration.Orchestrators;
using BabaFunkeAppRegistration.Tests.Helpers;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace BabaFunkeAppRegistration.Tests.BabaFunkeAppRegistration.Orchestrators
{
    public class RegisterAppUserOrchestratorFacts
    {
        private readonly RegisterAppUserOrchestrator _sut;
        private readonly Mock<IDurableOrchestrationContext> _orchestratonContext;
        private readonly Mock<ILogger> _logger;
        private AppUser _appUser;

        public RegisterAppUserOrchestratorFacts()
        {
            _sut = new RegisterAppUserOrchestrator();
            _orchestratonContext = new Mock<IDurableOrchestrationContext>();
            _logger = new Mock<ILogger>();
            _appUser = TestHelpers.GetAppuser();

            _orchestratonContext.Setup(o => o.GetInput<AppUser>()).Returns(_appUser);
            _orchestratonContext.Setup(o => o.CallActivityAsync<ActivityResponse>(It.IsAny<string>(), It.IsAny<AppUser>()))
               .ReturnsAsync(new ActivityResponse { IsSuccess = true });
        }

        [Fact]
        public async Task RegisterAppUser_ShouldCallValidateAppUserActivity()
        {
            await _sut.RegisterAppUser(_orchestratonContext.Object, _logger.Object);

            _orchestratonContext.Verify(o => o.CallActivityAsync<ActivityResponse>(StringConstants.ValidateAppUserActivity, It.IsAny<AppUser>()));
        }

        [Fact]
        public async Task RegisterAppUser_ShouldCallPersistAppUserActivity_IfIsSuccess()
        {
            await _sut.RegisterAppUser(_orchestratonContext.Object, _logger.Object);

            _orchestratonContext.Verify(o => o.CallActivityAsync<ActivityResponse>(StringConstants.PersistAppUserActivity, It.IsAny<AppUser>()));
        }

        [Fact]
        public async Task RegisterAppUser_ShouldCallSendConfirmationEmailActivity_IfIsSuccess()
        {
            await _sut.RegisterAppUser(_orchestratonContext.Object, _logger.Object);

            _orchestratonContext.Verify(o => o.CallActivityAsync<ActivityResponse>(StringConstants.SendConfirmEmailActivity, It.IsAny<AppUser>()));
        }

        [Fact]
        public async Task RegisterAppUser_ShouldCallCompleteRegistrationActivity_IfIsSuccess()
        {
            _orchestratonContext.Setup(o => o.CallActivityAsync<bool>(It.IsAny<string>(), It.IsAny<AppUser>()))
               .ReturnsAsync(true);

            await _sut.RegisterAppUser(_orchestratonContext.Object, _logger.Object);

            _orchestratonContext.Verify(o => o.CallActivityAsync<bool>(StringConstants.CompleteRegistrationActivity, It.IsAny<AppUser>()));
        }
    }
}
