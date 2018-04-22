using System.Collections.Generic;
using IdentityHost.Entities;

namespace IdentityHost.Services
{
    public interface IHostUserRepository
    {
        User GetUserByUsername(string username);
        User GetUserBySubjectId(string subjectId);
        User GetUserByEmail(string email);
        User GetUserByProvider(string loginProvider, string providerKey);
        IEnumerable<UserLogin> GetUserLoginsBySubjectId(string subjectId);
        IEnumerable<UserClaim> GetUserClaimsBySubjectId(string subjectId);
        bool AreUserCredentialsValid(string username, string password);
        bool IsUserActive(string subjectId);
        void AddUser(User user);
        void AddUserLogin(string subjectId, string loginProvider, string providerKey);
        void AddUserClaim(string subjectId, string claimType, string claimValue);
        bool Save();
    }
}
