using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SblendersAPI.Utils;

namespace SblendersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgenteTokenController : ControllerBase
    {
        [HttpGet("{login}/{pass}", Name = "GetAgentToken")]
        public Dictionary<string, string> Get(string login, string pass)
        {
            using (
               SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
               )
            using (
                SqlCommand idQueryCommand = new SqlCommand("SELECT agenteID, tipoAgenteID FROM tbAgente WHERE agenteLogin= @login AND agenteSenha = @pass ;", connection)
            )
            using (SqlDataAdapter idQueryAdapter = new SqlDataAdapter(idQueryCommand))
            {
                try
                {
                    idQueryCommand.Parameters.Add(new SqlParameter("@login", login));
                    idQueryCommand.Parameters.Add(new SqlParameter("@pass", PasswordHasher.Hash(pass, Program.hashSalt)));
                    connection.Open();
                    DataTable idQuery = new DataTable();
                    idQueryAdapter.Fill(idQuery);
                    if (idQuery.Rows.Count < 1)
                    {
                        Response.StatusCode = 403;
                        return new Dictionary<string, string> { { "error" , "AUTH_ERROR" } };
                    }
                    else
                    {
                        string agenteID = idQuery.Rows[0]["agenteID"].ToString();

                        if ((int)idQuery.Rows[0]["tipoAgenteID"] == 1)
                        {
                            using(SqlCommand verifyMailCommand = new SqlCommand("SELECT clienteOnlineVerificadoFlag FROM tbClienteOnline WHERE agenteID = @aid", connection))
                            {
                                DataTable clientVerifiedData = new DataTable();
                                verifyMailCommand.Parameters.Add(new SqlParameter("@aid", agenteID));

                                using(SqlDataAdapter d = new SqlDataAdapter(verifyMailCommand))
                                {
                                    d.Fill(clientVerifiedData);
                                    if (!(bool)clientVerifiedData.Rows[0]["clienteOnlineVerificadoFlag"])
                                    {
                                        Response.StatusCode = 403;
                                        return new Dictionary<string, string> { { "error", "EMAIL_NOT_VERIFIED_ERROR" } };
                                    }
                                }
                               
                            }
                        }
                        using (SqlCommand tokenUpdateCommand = new SqlCommand("UPDATE tbAgente SET agenteToken = @newToken WHERE agenteID = @id;", connection))
                        {
                            string newToken = RandomGenerator.GenerateHexString(32);               

                            tokenUpdateCommand.Parameters.Add(new SqlParameter("@newToken", newToken));
                            tokenUpdateCommand.Parameters.Add(new SqlParameter("@id", agenteID));

                            int rowsAffected = tokenUpdateCommand.ExecuteNonQuery();

                            if (rowsAffected < 1)
                            {
                                Response.StatusCode = 500;
                                return new Dictionary<string, string> { { "error", "TOKEN_GENERATE_ERROR" } };
                            }
                            else
                            {
                                return new Dictionary<string, string> { {"id", agenteID }
                                , {"token", newToken} };
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    
                    Response.StatusCode = 500;
                    return new Dictionary<string, string> { { "error", "SERVICE_NOT_AVAIBLE_ERROR" }, {"debugInfo", e.Message } };
                }

            }
        }
    }
}