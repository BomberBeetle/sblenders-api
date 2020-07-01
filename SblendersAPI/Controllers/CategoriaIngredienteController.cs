using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SblendersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaIngredienteController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetCategoriaNome")]
        public string Get(int id)
        {
            using (
                SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
            )
            using (
               SqlCommand selectCategoriaCommand = new SqlCommand("SELECT categoriaIngredienteNome FROM tbCategoriaIngrediente WHERE categoriaIngredienteID = @id", connection)
                )
            {
                selectCategoriaCommand.Parameters.Add(new SqlParameter("@id", id));
                connection.Open();
                return (string)selectCategoriaCommand.ExecuteScalar();
            }
        }
    }
}
