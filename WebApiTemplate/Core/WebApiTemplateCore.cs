using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebApiTemplate.Core
{
    public class WebApiTemplateCore
    {
        private readonly ILogger _logger;

        public WebApiTemplateCore(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> GetSomeData([FromBody] string text)
        {
            return "Passed value: " + text;
        }

        public async Task<IEnumerable<int>> GetSomeData()
        {
            return Enumerable.Range(1, 5);
        }
    }
}