using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        public Dictionary<string, object> Get(string userid, string clientid)
        {
            string token = Request.Headers["Authorization"];

            using (
              SqlConnection connection = new SqlConnection(string.Format(
                  "User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}"
                  , Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
              )

            using (
                SqlCommand userQueryCommand = new SqlCommand(
                    "SELECT agenteID, tipoAgenteID FROM tbAgente WHERE agenteID= @id AND agenteToken = @token;"
                    , connection)
            )
            {

                userQueryCommand.Parameters.Add(new SqlParameter("@token", token));
                userQueryCommand.Parameters.Add(new SqlParameter("@id", userid));

                using (SqlDataAdapter userQueryAdapter = new SqlDataAdapter(userQueryCommand))
                {
                    DataTable userQuery = new DataTable();
                    userQueryAdapter.Fill(userQuery);

                    if (userQuery.Rows.Count < 1)
                    {
                        Response.StatusCode = 403;
                        return new Dictionary<string, object>
                    {
                        { "error", "AUTH_ERROR" }
                    };
                    }

                    if ((int)userQuery.Rows[0]["tipoAgenteID"] == 2) //tem que ser funcionario
                    {
                        using (SqlCommand clientQueryCommand = new SqlCommand(
                            "SELECT clienteOnlineNome, clienteOnlineSobrenome FROM tbClienteOnline WHERE agenteID= @id;",
                            connection))
                        {
                            clientQueryCommand.Parameters.Add(new SqlParameter("@id", clientid));

                            using (SqlDataAdapter clientQueryAdapter = new SqlDataAdapter(clientQueryCommand))
                            {
                                DataTable clientTable = new DataTable();
                                clientQueryAdapter.Fill(clientTable);

                                if (clientTable.Rows.Count < 1)
                                {
                                    Response.StatusCode = 404;
                                    return new Dictionary<string, object>
                                    {
                                        { "error", "CLIENT_NOT_FOUND_ERROR" }
                                    };
                                }

                                else
                                {
                                    return new Dictionary<string, object>
                                    {
                                        {"clientName", clientTable.Rows[0]["clienteOnlineNome"].ToString()},
                                        {"clientSurname", clientTable.Rows[0]["clienteOnlineSobrenome"].ToString()}
                                    };
                                }

                            }
                        }
                    }
                    Response.StatusCode = 403;
                    return new Dictionary<string, object>
                    {
                        { "error", "AUTH_ERROR" }
                    };
                }  
            }
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
