using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleInjector;
using WebApiTemplate.Core;

namespace WebApiTemplate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebApiTemplateController : ControllerBase
    {
        private readonly WebApiTemplateCore _core;
        private readonly Container  _serviceContainer;

        public WebApiTemplateController(Container serviceContainer)
        {
            _serviceContainer = serviceContainer;

            _core = _serviceContainer.GetInstance<WebApiTemplateCore>();
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