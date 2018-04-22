using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityHost.Entities;
using IdentityHost.Services;
using IdentityModel;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityHost.Controllers.UserRegistration
{
    public class UserRegistrationController : Controller
    {
        private readonly IHostUserRepository _userRepository;
        private readonly IIdentityServerInteractionService _interactionService;

        public UserRegistrationController(IHostUserRepository userRepository,
            IIdentityServerInteractionService interactionService)
        {
            _userRepository = userRepository;
            _interactionService = interactionService;
        }


        [HttpGet]
        public IActionResult RegisterUser(RegistrationInputModel registrationInputModel)
        {
            var vm = new RegisterUserViewModel
            {
                ReturnUrl = registrationInputModel.ReturnUrl,
                Provider = registrationInputModel.Provider,
                ProviderUserId = registrationInputModel.ProviderUserId
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userToCreate = new User
                {
                    Password = model.Password,
                    Username = model.Username,
                    IsActive = true,

                    Claims = new List<UserClaim>
                    {
                        new UserClaim("country", model.Country),
                        new UserClaim(JwtClaimTypes.Address, model.Address),
                        new UserClaim(JwtClaimTypes.GivenName, model.Firstname),
                        new UserClaim(JwtClaimTypes.FamilyName, model.Lastname),
                        new UserClaim(JwtClaimTypes.Email, model.Email),
                        new UserClaim("subscription", "freeuser")
                    }
                };

                if (model.IsProvisioningFromExternal)
                {
                    userToCreate.Logins.Add(new UserLogin
                    {
                        LoginProvider = model.Provider,
                        ProviderKey = model.ProviderUserId,

                    });
                }

                _userRepository.AddUser(userToCreate);

                if (!_userRepository.Save())
                    throw new Exception("Creating a user failed.");

                if (!model.IsProvisioningFromExternal)
                {
                    // Log in the newly created user

                    await HttpContext.SignInAsync(userToCreate.SubjectId, userToCreate.Username);
                }

                

                if (_interactionService.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return Redirect("~/");

            }

            // If we got this far, something failed
            return View(model);
        }
    }
}