using Microsoft.AspNetCore.Mvc;

namespace BootCom.MongoDB.Sync.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {

        internal readonly ILogger _logger;

        public BaseController(ILogger logger)
        {
            _logger = logger;
        }

    }
}
