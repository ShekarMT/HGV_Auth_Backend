using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using HGVServiceAuth.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace HGVServiceAuth.Controllers
{
    [Authorize(Roles = "InventoryManager,PremiumAgent")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        [Route("GetClaimListForUser")]
        public async Task<IActionResult> Get()
        {

            try
            {
                var graphserviceClient = GraphServiceProvider.ClientProvider();
                
                if(graphserviceClient != null)
                {
                    var userInfo = await graphserviceClient.Users.Request().Select("country,city,state,displayname,userprincipalname").GetAsync();

                    List<string> claimsList = new List<string>();
                    var claims = HttpContext.User.Claims;
                    foreach (var claim in claims)
                    {
                        claimsList.Add(claim.Type + "->" + claim.Value);
                        if (claim.Type.Contains("upn"))
                        {
                            foreach (var user in userInfo)
                            {
                                if (user.UserPrincipalName.Equals(claim.Value))
                                {
                                    claimsList.Add("Country -> " + user.Country);
                                    claimsList.Add("City -> " + user.City);
                                    break;
                                }
                            }
                        }
                    }
                    return Ok(claimsList);
                }
                return BadRequest();

            }
            catch(Exception ex)
            {
                return BadRequest();
            }
            
        }

        // GET api/values/5
        [HttpGet]
        [Route("GetUserRole")]
        public async Task<IActionResult> GetClaim()
        {
            try
            {
                var claims = HttpContext.User.Claims;
                foreach (var claim in claims)
                {
                    if (claim.Type.Contains("role"))
                    {
                        return Ok(claim.Value.ToString());
                    }
                }
                return NotFound("Not Assigned");
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
