using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers
{
    
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DefaultController : ControllerBase
    {
        [Route("/")]
        public RedirectResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}