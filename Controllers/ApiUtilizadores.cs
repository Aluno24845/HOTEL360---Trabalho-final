using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HOTEL360___Trabalho_final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUtilizadores : ControllerBase
    {
        // GET: api/<ApiUtilizadores>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ApiUtilizadores>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ApiUtilizadores>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ApiUtilizadores>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ApiUtilizadores>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
