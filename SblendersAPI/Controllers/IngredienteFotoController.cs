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
    public class IngredienteFotoController : ControllerBase
    {
        // GET: api/IngredienteFoto/5
        [HttpGet("{id}", Name = "Get")]
        public HttpResponseMessage Get(int id)
        {
            using (
                SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
            )
            using (
               SqlCommand selectIngredientPhotoCommand = new SqlCommand("SELECT ingredienteFoto FROM tbIngrediente WHERE ingredienteID = @id", connection)
                )
            {
                selectIngredientPhotoCommand.Parameters.Add(new SqlParameter("@id", id));
                byte[] photoFile = (byte[])selectIngredientPhotoCommand.ExecuteScalar();
                HttpResponseMessage responseMessage = new HttpResponseMessage();
                responseMessage.Content = new ByteArrayContent(photoFile);
                responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                return responseMessage;
            }
        }
    }
}
