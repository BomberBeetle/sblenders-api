using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SblendersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteOnlineController : ControllerBase
    {
        // GET: api/ClienteOnline
        [HttpGet("{userid}/{clientid}", Name = "GetOnlineClient")]
        public IEnumerable<string> Get()
        {

            return new string[] {};
           
        }


        // POST: api/ClienteOnline
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/ClienteOnline/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
