using System;
using System.Collections.Generic;
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
    public class AutenticaClienteOnlineController : ControllerBase
    {

        // POST: api/AutenticaClienteOnline
        [HttpPost("{id}/{url}")]
        public string Post(int id, string url)
        {
            using (
             SqlConnection connection = new SqlConnection(string.Format(
                 "User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}"
                 , Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
             )

            using (
                SqlCommand authClientCommand = new SqlCommand(
                    "UPDATE tbClienteOnline SET clienteOnlineVerificadoFlag = 1, clienteOnlineUrlVerifica = NULL WHERE agenteID = @id AND clienteOnlineUrlVerifica = @url"
                    , connection)
            )
            {
                authClientCommand.Parameters.Add(new SqlParameter("@id", id));
                authClientCommand.Parameters.Add(new SqlParameter("@url", url));
                connection.Open();
                int rowsAffected = authClientCommand.ExecuteNonQuery();
                if(rowsAffected < 1)
                {
                    Response.StatusCode = StatusCodes.Status403Forbidden;
                    return "AUTH_ERROR";
                }
                else
                {
                    //make new token
                    using (
                SqlCommand generateTokenCommand = new SqlCommand(
                    "UPDATE tbAgente SET agenteToken = @token WHERE agenteID = @id"
                    , connection)
                )
                    {
                        string token = RandomGenerator.GenerateHexString(32);
                        generateTokenCommand.Parameters.Add(new SqlParameter("@id", id));
                        generateTokenCommand.Parameters.Add(new SqlParameter("@token", token));
                        generateTokenCommand.ExecuteNonQuery();
                        return token;
                    }
                }
            }
        }
    }
}
