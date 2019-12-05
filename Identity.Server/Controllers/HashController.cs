using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Models;

namespace Identity.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HashController : ControllerBase
    {
        [HttpPost]
        public ActionResult GetPassowrdHash(
            Hash hash)
            => new OkObjectResult(hash.Password.Sha256());

        [HttpGet]
        public ActionResult HealthStatus()
            => Ok();

        public class Hash
        {
            public string Password { get; set; }
        }
    }
}