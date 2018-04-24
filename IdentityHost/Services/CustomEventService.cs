using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace IdentityHost.Services
{
    public class CustomEventService : DefaultEventService
    {
        public CustomEventService(IdentityServerOptions options, IHttpContextAccessor httpContextAccessor,
            IEventSink eventSink, ISystemClock systemClock) 
            : base(options, httpContextAccessor, eventSink, systemClock)
        {
        }

        protected override async Task PrepareEventAsync(Event evt)
        {
            await base.PrepareEventAsync(evt);

            switch (evt.Id)
            {
                case EventIds.TokenIssuedSuccess:
                    // Save Core token
                    break;


                case EventIds.TokenRevokedSuccess:
                    // Revoke Core token
                    break;

            }
        }
    }
}