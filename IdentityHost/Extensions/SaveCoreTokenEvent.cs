using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Events;

namespace IdentityHost.Extensions
{
    public class SaveCoreTokenEvent : UserLoginSuccessEvent
    {
        private readonly string _clientId;
        private readonly IServiceProvider _serviceProvider;

        public SaveCoreTokenEvent(string provider, string providerUserId, string subjectId, string clientId,
            IServiceProvider serviceProvider) : base(provider,
            providerUserId, subjectId)
        {
            _clientId = clientId;
            _serviceProvider = serviceProvider;
        }

        protected override Task PrepareAsync()
        {
            // Save Host Token here

            var grantDbContext = (PersistedGrantDbContext) _serviceProvider.GetService(typeof(PersistedGrantDbContext));

            grantDbContext.PersistedGrants.Add(new PersistedGrant
            {
                Type = "core_token",
                ClientId = _clientId,
                SubjectId = SubjectId,
                Key = Guid.NewGuid().ToString(),
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.MaxValue,
                Data = "core-token-goes-here"
            });


            grantDbContext.SaveChanges();

            return Task.CompletedTask;
        }
    }
}