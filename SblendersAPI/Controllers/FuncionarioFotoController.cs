using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SblendersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuncionarioFotoController : ControllerBase
    {
        // GET: api/FuncionarioFoto/5
        [HttpGet("{id}", Name = "Get")]
        public HttpResponseMessage Get(int id)
        {
            using (
                SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
            )
            using (
                SqlCommand authEmpCommand = new SqlCommand("SELECT agenteID FROM tbAgente WHERE agentID = @id AND agentToken=@token AND tipoFuncionarioID = 2", connection)
            )
            {
                connection.Open();
                int? agentID = (int?)authEmpCommand.ExecuteScalar();
                if(agentID == null)
                {
                    Response.StatusCode = 403;
                    return new HttpResponseMessage();
                }
                using (
                SqlCommand selectEmpPhotoCommand = new SqlCommand("SELECT funcionarioFoto FROM tbFuncionario WHERE agentID = @id", connection)
                 )
                {
                    selectEmpPhotoCommand.Parameters.Add(new SqlParameter("@id", id));
                    byte[] photoFile = (byte[])selectEmpPhotoCommand.ExecuteScalar();
                    HttpResponseMessage responseMessage = new HttpResponseMessage();
                    responseMessage.Content = new ByteArrayContent(photoFile);
                    responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                    return responseMessage;
                }
            }
        }
    }
}
