using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApiTemplate.Core;

namespace WebApiTemplate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebApiTemplateController : ControllerBase
    {
        private WebApiTemplateCore _core;

        public WebApiTemplateController()
        {
            _core = new();
        }

        [HttpPost]
        [Route("TryPost")]
        public async Task<object> GetSomeData([FromBody] string text)
            => await _core.GetSomeData(text);

        [HttpGet]
        [Route("TryGet")]
        public async Task<IEnumerable<int>> GetSomeData()
            => await _core.GetSomeData();
    }
}