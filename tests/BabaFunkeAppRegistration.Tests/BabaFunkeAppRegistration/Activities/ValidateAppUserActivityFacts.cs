using BabaFunkeAppRegistration.Activities;
using BabaFunkeAppRegistration.Services;
using Moq;
using Shared;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BabaFunkeAppRegistration.Tests.BabaFunkeAppRegistration.Activities
{
    public class ValidateUserActivityFacts
    {
        private readonly ValidateAppUserActivity _sut;
        private readonly Mock<IValidationService> _validationService;

        public ValidateUserActivityFacts()
        {
            _validationService = new Mock<IValidationService>();
            _sut = new ValidateAppUserActivity(_validationService.Object);
        }

        [Fact]
        public void ValidateUserActivity_ShouldThrowException_IfNullConstructorArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new ValidateAppUserActivity(null));
        }

        [Fact]
        public async Task RunAsync_ShouldCallService()
        {
            await _sut.RunAsync(new AppUser());

            _validationService.Verify(v => v.IsValidated(It.IsAny<AppUser>()), Times.Once);
        }

        [Fact]
        public async Task RunAsync_ShouldCallActivityResponse()
        {
            _validationService.Setup(v => v.IsValidated(It.IsAny<AppUser>())).Returns(new ActivityResponse());

            var result = await _sut.RunAsync(new AppUser());

            Assert.IsType<ActivityResponse>(result);
        }
    }
}