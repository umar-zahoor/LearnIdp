using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ImageGallery.Client.Services
{
    public class ImageGalleryHttpClient : IImageGalleryHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IConfiguration _configuration;

        public ImageGalleryHttpClient(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        
        public async Task<HttpClient> GetClient()
        {
            string accessToken;
            
            var apiUrl = _configuration.GetValue<string>("AppSettings:Api:Uri");
            _httpClient.BaseAddress = new Uri(apiUrl);

            //var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var expiresAt = await _httpContextAccessor.HttpContext.GetTokenAsync("expires_at");

            if (string.IsNullOrWhiteSpace(expiresAt) ||
                DateTime.Parse(expiresAt).AddSeconds(-60).ToUniversalTime() < DateTime.UtcNow)
            {
                accessToken = await RenewTokens();
            }
            else
            {
                accessToken =
                    await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }

            if(!string.IsNullOrWhiteSpace(accessToken))
                _httpClient.SetBearerToken(accessToken);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }

        public async Task<string> RenewTokens()
        {
            var currentContext = _httpContextAccessor.HttpContext;
            
            // Get IDP enpoint address
            var authority = _configuration.GetValue<string>("AppSettings:Idp:Uri");
            
            // Get the endpoint metadata
            var discoveryClient = new DiscoveryClient(authority);
            var discoveryResponse = await discoveryClient.GetAsync();
            
            // Create a new token client to get new tokens
            var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "ImageGallery-WebApp", "secret");

            var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

            if (!tokenResult.IsError)
            {
                var authenticateInfo = await currentContext.AuthenticateAsync("Cookies");
                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);

                authenticateInfo.Ticket.Properties.UpdateTokenValue("expires_at",
                    expiresAt.ToString("o", CultureInfo.InvariantCulture));

                authenticateInfo.Ticket.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken,
                    tokenResult.AccessToken);

                authenticateInfo.Ticket.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken,
                    tokenResult.RefreshToken);

                await currentContext.SignInAsync("Cookies", authenticateInfo.Principal, authenticateInfo.Properties);

                return tokenResult.AccessToken;
            }

            throw new Exception("Problem occured while refreshing tokens.", tokenResult.Exception);

        }
    }
}

