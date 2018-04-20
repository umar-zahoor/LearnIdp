using System;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ImageGallery.Api.Authorization
{
    public class MustOwnImageHandler : AuthorizationHandler<MustOwnImageRequirement>
    {
        private readonly IGalleryRepository _galleryRepository;
        public MustOwnImageHandler(IGalleryRepository galleryRepository)
        {
            _galleryRepository = galleryRepository;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustOwnImageRequirement requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;

            if (filterContext == null)
            {
                context.Fail();
                return Task.FromResult(0);
            }

            filterContext.RouteData.Values.TryGetValue("id", out var idValue);

            if (!Guid.TryParse(idValue?.ToString(), out var imageId))
            {
                context.Fail();
                return Task.FromResult(0);
            }

            // Get sub claim
            var ownerId = context.User.Claims?.FirstOrDefault(_ => _.Type == "sub")?.Value;

            if (!_galleryRepository.IsImageOwner(imageId, ownerId))
            {
                context.Fail();
                return Task.FromResult(0);
            }

            // If all passes, mark requirement as passed
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}