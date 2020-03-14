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
    public class AddressController : BaseController
    {
        private readonly IAccountService userService;
        private readonly IAddressService addressService;
        private readonly ILogger<AddressController> logger;

        public AddressController(
            IOptions<AppSettings> _appSettings,
            IHttpContextAccessor _httpContextAccessor,
            IAccountService _userService,
            IAddressService _addressService,
            ILogger<AddressController> _logger) : base(_appSettings, _httpContextAccessor, _userService)
        {
            logger = _logger;
            userService = _userService;
            addressService = _addressService;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
