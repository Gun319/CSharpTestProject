using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionFilter.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string Test1()
        {
            //string str = System.IO.File.ReadAllText("f:/1.txt");
            Console.WriteLine("action 代码");
            return "123";
        }
    }
}
