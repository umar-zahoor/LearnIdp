using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ImageGallery.Api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IConfiguration _configuration;

        public ValuesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        // GET api/values
        [HttpGet]
        //[Authorize]
        public async Task<JsonResult> Get()
        {
            //var discoveryClient = new DiscoveryClient(_configuration.GetValue<string>("AppSettings:Idp:Uri"));
            //var response = await discoveryClient.GetAsync();

            //var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            //var userInfoClient = new UserInfoClient(response.UserInfoEndpoint);

            //var userInfoResponse = await userInfoClient.GetAsync(accessToken);
            //var claims = userInfoResponse.Claims;

            //return new JsonResult(claims);

            return new JsonResult(new List<string> { "value1", "value2" });

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
