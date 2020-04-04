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

        [HttpGet("{id}", Name = "GetAgente")]
        public Dictionary<string, string> Get(int id, string token)
        {
            
            using (
              SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
              )
            using (
                SqlCommand agenteQueryCommand = new SqlCommand("SELECT tipoAgenteID FROM tbAgente WHERE agenteToken = @token AND agenteID = @id ;", connection)
            )
            using (SqlDataAdapter agenteQueryAdapter = new SqlDataAdapter(agenteQueryCommand))
            {
                try
                {
                    agenteQueryCommand.Parameters.Add(new SqlParameter("@token", Request.Headers["Authorization"].ToString()));
                    agenteQueryCommand.Parameters.Add(new SqlParameter("@id", id));
                    connection.Open();
                    DataTable agenteQuery = new DataTable();
                    agenteQueryAdapter.Fill(agenteQuery);

                    if((int)agenteQuery.Rows[0]["tipoAgenteID"] == 0)
                    {
                        Response.StatusCode = 418;
                        return new Dictionary<string, string> { { "message", "I'm a teapot!" } };
                    }
                    else if((int)agenteQuery.Rows[0]["tipoAgenteID"] == 1)
                    {
                        //ClienteOnline
                        using (SqlCommand clienteOnlineQueryCommand = new SqlCommand("SELECT * FROM tbClienteOnline where agenteID = @id", connection))

                        using (SqlDataAdapter clienteOnlineQueryAdapter = new SqlDataAdapter(clienteOnlineQueryCommand))
                        {
                            clienteOnlineQueryCommand.Parameters.Add(new SqlParameter("@id", id));
                            DataTable clienteOnlineQuery = new DataTable();
                            clienteOnlineQueryAdapter.Fill(clienteOnlineQuery);
                            return new Dictionary<string, string> {
                                { "client_id", clienteOnlineQuery.Rows[0]["clienteOnlineID"].ToString()},
                                { "client_name", clienteOnlineQuery.Rows[0]["clienteOnlineNome"].ToString() },
                                { "client_surname", clienteOnlineQuery.Rows[0]["clienteOnlineSobrenome"].ToString()}
                            };
                        }
                        
                        
                    }
                    else if ((int)agenteQuery.Rows[0]["tipoAgenteID"] == 2)
                    {
                        //Funcionario
                        using (SqlCommand funcionarioQueryCommand = new SqlCommand("SELECT restauranteID, tipoFuncionarioID, nomeFuncionario, idFuncionario FROM tbFuncionario where agenteID = @id", connection))

                        using (SqlDataAdapter funcionarioQueryAdapter = new SqlDataAdapter(funcionarioQueryCommand))
                        {
                            funcionarioQueryCommand.Parameters.Add(new SqlParameter("@id", id));
                            DataTable funcionarioQuery = new DataTable();
                            funcionarioQueryAdapter.Fill(funcionarioQuery);
                            return new Dictionary<string, string> {
                                { "restaurant_id", funcionarioQuery.Rows[0]["restauranteID"].ToString()},
                                { "emp_type_id", funcionarioQuery.Rows[0]["tipoFuncionarioID"].ToString() },
                                { "emp_name", funcionarioQuery.Rows[0]["nomeFuncionario"].ToString()},
                                {"emp_id",  funcionarioQuery.Rows[0]["idFuncionario"].ToString() }
                            };
                        }
                    }
                    else if ((int)agenteQuery.Rows[0]["tipoAgenteID"] == 3)
                    {
                        //ThotEm
                    }
                }
                catch(Exception e)
                {
                    return new Dictionary<string, string> { { "error", "AUTH_ERROR" }, { "debugInfo", e.Message } };
                }
            }

            Response.StatusCode = 500;
            return new Dictionary<string, string> { {"error", "SERVICE_NOT_AVAIBLE" } };
        }
    }
}
