using Microsoft.AspNetCore.Mvc;

namespace Todo.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class PingsController : ControllerBase
    {
        [HttpGet("ping")]
        public void Ping()
        {
        }
    }
}