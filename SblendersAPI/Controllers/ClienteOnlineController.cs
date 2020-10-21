using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SblendersAPI.Models;
using SblendersAPI.Utils;

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
                connection.Open();
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
        [HttpPut]
        public Dictionary<string, object> Put([FromBody] ClienteOnline newClient)
        {
            
            if(newClient.Login == null || newClient.Password == null || newClient.Nome == null || newClient.Sobrenome == null)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return new Dictionary<string, object> {
                    {"error", "MALFORMED_REQUEST_ERROR" },
                };
            }

            if (!EmailChecker.IsValidEmail(newClient.Login))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return new Dictionary<string, object> {
                    {"error", "EMAIL_INVALID_ERROR" },
                };
            }

            else if(newClient.Password.Length < 1){
                return new Dictionary<string, object> {
                    {"error", "PASS_TOO_SHORT_ERROR" },
                };
            }

            else if (newClient.Nome.Length < 1)
            {
                return new Dictionary<string, object> {
                    {"error", "NAME_TOO_SHORT_ERROR" },
                };
            }

            else if (newClient.Sobrenome.Length < 1)
            {
                return new Dictionary<string, object> {
                    {"error", "SURNAME_TOO_SHORT_ERROR" },
                };
            }

            else
            {
                using (
                    SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
                )
                using (
                   SqlCommand insertAgentCommand = new SqlCommand("INSERT INTO tbAgente(tipoAgenteID, agenteLogin, agenteSenha, agenteSalt) VALUES(1, @login, @pass, @salt) SELECT CAST(SCOPE_IDENTITY() AS INT)", connection)
                )
                {
                    string salt = RandomGenerator.GenerateHexString(32);
                    insertAgentCommand.Parameters.Add(new SqlParameter("@salt", salt));
                    insertAgentCommand.Parameters.Add(new SqlParameter("@login", newClient.Login));
                    insertAgentCommand.Parameters.Add(new SqlParameter("@pass", PasswordHasher.Hash(newClient.Password, salt)));
                    connection.Open();
                    int agentID;
                    try
                    {
                        agentID = (int)insertAgentCommand.ExecuteScalar();
                    }

                    catch (SqlException ex)
                    {
                        if (ex.Number == 2601 || ex.Number == 2627) //ver se é unique violation
                        {
                            Response.StatusCode = StatusCodes.Status400BadRequest;
                            return new Dictionary<string, object> { { "error", "LOGIN_ALREADY_EXISTS_ERROR" } };
                        }
                        else
                        {
                            Response.StatusCode = StatusCodes.Status500InternalServerError;
                            return new Dictionary<string, object> { { "error", "INTERNAL_ERROR" } };
                        }
                    }

                    using (
                        SqlCommand insertClientCommand = new SqlCommand("INSERT INTO tbClienteOnline(clienteOnlineNome, clienteOnlineSobrenome, clienteOnlineUrlVerifica, clienteOnlineVerificadoFlag, agenteID) VALUES(@name, @surname, @url, 0, @id)", connection)
                        )
                        {
                        string url = RandomGenerator.GenerateHexString(16);    

                            insertClientCommand.Parameters.Add(new SqlParameter("@name", newClient.Nome));
                            insertClientCommand.Parameters.Add(new SqlParameter("@surname", newClient.Sobrenome));
                            insertClientCommand.Parameters.Add(new SqlParameter("@url", url));
                            insertClientCommand.Parameters.Add(new SqlParameter("@id", agentID));

                            int rowsAffected = insertClientCommand.ExecuteNonQuery();
                            if (rowsAffected < 1)
                            {
                                Response.StatusCode = StatusCodes.Status500InternalServerError;
                                return new Dictionary<string, object> { { "error", "INTERNAL_ERROR" } };
                            }
                            else
                            {
                                //mandar email aqui
                                return new Dictionary<string, object> { { "message", "SUCCESS" } };
                            }
                        }
                    
                    
                }
            }

        }
        [HttpPost("{id}")]
        public void Post([FromBody] ClienteOnline new_info, int id){

            string token = Request.Headers["Authorization"];

            using (
              SqlConnection connection = new SqlConnection(string.Format(
                  "User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}"
                  , Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
              )

            using (
                SqlCommand userQueryCommand = new SqlCommand(
                    "SELECT agenteID, tipoAgenteID, agenteSalt FROM tbAgente WHERE agenteID= @id AND agenteToken = @token;"
                    , connection)
            ){
                userQueryCommand.Parameters.Add(new SqlParameter("@id", id));
                userQueryCommand.Parameters.Add(new SqlParameter("@token", token));
                using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(userQueryCommand)){
                    var authTable = new DataTable();
                    sqlAdapter.Fill(authTable);
                    if(authTable.Rows.Count != 1){
                        Response.StatusCode = 403;
                        return;
                    }
                    else if((int)authTable.Rows[0]["tipoAgenteID"] != 1){
                        Response.StatusCode = 403;
                        return;
                    }
                    else{
                        using(SqlCommand changeInfoCommand = new SqlCommand()){
                        List<string> updateCommands = new List<string>();
                        if(new_info.Nome != null){
                            updateCommands.Add("SET tbClienteOnline.clienteOnlineNome = @nome");
                            changeInfoCommand.Parameters.Add(new SqlParameter("@nome", new_info.Nome));
                        }
                        if(new_info.Sobrenome != null){
                            updateCommands.Add("SET tbClienteOnline.clienteOnlineSobrenome = @sbnome");
                            changeInfoCommand.Parameters.Add(new SqlParameter("@sbnome", new_info.Sobrenome));
                        }
                        if(new_info.Password != null){
                            updateCommands.Add("SET tbAgente.agenteSenha = @pass");
                            changeInfoCommand.Parameters.Add(new SqlParameter("@pass", PasswordHasher.Hash(new_info.Password, authTable.Rows[0]["agenteSalt"].ToString())));
                        }
                        changeInfoCommand.Parameters.Add(new SqlParameter("@id", id));
                        changeInfoCommand.CommandText = $"UPDATE tbClienteOnline {String.Join(",",updateCommands.ToArray())} FROM tbClienteOnline INNER JOIN on tbAgente ON tbAgente.agenteID = tbClienteOnline.agenteID WHERE tbClienteOnline.agenteID = @id";
                        changeInfoCommand.Connection = connection;
                        changeInfoCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (
              SqlConnection connection = new SqlConnection(string.Format(
                  "User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}"
                  , Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
              )

            using (
                SqlCommand deleteClientCommand = new SqlCommand(
                    "DELETE FROM tbClienteOnline WHERE agenteID = @id AND agenteToken = @token"
                    , connection)
            )
            {
                deleteClientCommand.Parameters.Add(new SqlParameter("@id", id));
                deleteClientCommand.Parameters.Add(new SqlParameter("@token", Request.Headers["Authorization"].ToString()));
                connection.Open();
                int rowsAffected = deleteClientCommand.ExecuteNonQuery();
                if(rowsAffected < 1)
                {
                    Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
                else
                {
                    using (
                SqlCommand deleteAgentCommand = new SqlCommand(
                    "DELETE FROM tbAgente WHERE agenteID = @id AND agenteToken = @token"
                    , connection)
                    )
                    {
                        deleteAgentCommand.Parameters.Add(new SqlParameter("@id", id));
                        deleteAgentCommand.Parameters.Add(new SqlParameter("@token", Request.Headers["Authorization"].ToString()));
                        deleteAgentCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
