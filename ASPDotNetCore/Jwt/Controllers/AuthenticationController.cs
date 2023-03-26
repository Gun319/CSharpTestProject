using Jwt.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jwt.Controllers
{

    [ApiController, Route("[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly JwtImplementation _jwtImplementation;

        public AuthenticationController(JwtImplementation jwtImplementation)
        {
            _jwtImplementation = jwtImplementation;
        }

        [HttpGet]
        public ActionResult<string> GetToken()
        {
            List<string> roles = new()
            {
                "admin","普通"
            };

            List<string> permissons = new()
            {
                "User.Create"
            };
            return _jwtImplementation.CreateToken("jj", roles, permissons);
        }

        [HttpGet]
        [Authorize(Permissions.UserCreate)]
        public ActionResult<string> GetTest()
        {
            return "Test Authorize";
        }
    }
}
