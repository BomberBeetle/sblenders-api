using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;

namespace SblendersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgenteController : ControllerBase
    {   

        [HttpGet("{id}/{token}", Name = "GetAgente")]
        public Dictionary<string, string> Get(string token)
        {
            /*
            using (
              SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
              )
            using (
                SqlCommand agenteQueryCommand = new SqlCommand("SELECT agente FROM tbAgente WHERE agenteLogin= @login AND agenteSenha = @pass ;", connection)
            )
            using (SqlDataAdapter agenteQueryAdapter = new SqlDataAdapter(agenteQueryCommand))
            {
                try
                {
                    agenteQueryCommand.Parameters.Add(new SqlParameter("@login"));
                    agenteQueryCommand.Parameters.Add(new SqlParameter("@pass")));
                    connection.Open();
                    DataTable idQuery = new DataTable();
                    agenteQueryAdapter.Fill(idQuery);
                }
                catch
                {

                }
            }
            */
            return new Dictionary<string, string> { {"error", "SERVICE_NOT_IMPLEMENTED" } };
        }

        // POST: api/Agente
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Agente/5
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
