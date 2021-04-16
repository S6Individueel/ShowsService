using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowsService.Controllers
{
    [Route("home")]
    [ApiController]
    public class HomeController : Controller
    {
        [HttpGet("test")]
        public String TestAsync()
        {
            Console.WriteLine("HELLO WORLD!!");
            return "Hello World"; 
        }
    }
}
