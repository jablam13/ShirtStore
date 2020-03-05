using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreModel.Generic;
using StoreService.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShirtStoreService.Controllers
{
    [Route("api/[controller]")]
    public class VisitorController : BaseController
    {
        private readonly IAccountService userService;
        private readonly ILogger<AccountController> logger;

        public VisitorController(
            IOptions<AppSettings> _appSettings,
            IHttpContextAccessor _httpContextAccessor,
            IAccountService _userService,
            ILogger<AccountController> _logger) : base(_appSettings, _httpContextAccessor, _userService)
        {
            logger = _logger;
            userService = _userService;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<Guid> Get()
        {
            return await userService.GetVisitorUid();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
