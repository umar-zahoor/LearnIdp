using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityHost.Services
{
    public class HostUserProfileService : IProfileService
    {
        private readonly IHostUserRepository _userRepository;

        public HostUserProfileService(IHostUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var userClaims = _userRepository.GetUserClaimsBySubjectId(subjectId);

            context.IssuedClaims = userClaims
                .Select(_ => new Claim(_.ClaimType, _.ClaimValue)).ToList();

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            context.IsActive = _userRepository.IsUserActive(subjectId);

            return Task.FromResult(0);
        }
    }
}