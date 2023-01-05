using BabaFunkeAppRegistration.Services;
using Shared;
using Xunit;

namespace BabaFunkeAppRegistration.Tests.BabaFunkeAppRegistration.Services
{
    public class ValidationServiceFacts
    {
        private readonly ValidationService _sut;

        public ValidationServiceFacts()
        {
            _sut = new ValidationService();
        }

        [Fact]
        public void IsValidated_ShouldReturnResponse_IfUsernameIsEmpty()
        {
            var appUser = new AppUser
            {
                Username = ""
            };

            var result = _sut.IsValidated(appUser);

            Assert.Equal("The username is missing.", result.Message);
        }

        [Fact]
        public void IsValidated_ShouldReturnResponse_IfEmailIsEmpty()
        {
            var appUser = new AppUser
            {
                Username = "Bayo",
                Email = ""
            };

            var result = _sut.IsValidated(appUser);

            Assert.Equal("The Email is missing.", result.Message);
        }

        [Fact]
        public void IsValidated_ShouldReturnResponse_IfEmailIsInvalid()
        {
            var appUser = new AppUser
            {
                Username = "Bayo",
                Email = "fakeemail@example"
            };

            var result = _sut.IsValidated(appUser);

            Assert.Equal("The Email address is invalid.", result.Message);
        }

        [Fact]
        public void IsValidated_ShouldReturnSuccessResponse_IfSuccessful()
        {
            var appUser = new AppUser
            {
                Username = "BabaFunke",
                Email = "goodemail@example.com"
            };

            var result = _sut.IsValidated(appUser);

            Assert.True(result.IsSuccess);
        }
    }
}