using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SblendersAPI.Models;

namespace SblendersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestauranteController : ControllerBase
    {
        // GET: api/Restaurante
        [HttpGet("{lat}/{lng}/{range}")]
        public IEnumerable<Restaurante> Get(decimal lat, decimal lng, decimal range)
        {
            DataTable dt = new DataTable();
            using (
             SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
            )
            {
                using (SqlCommand restauranteSelectCommand = new SqlCommand("SELECT * FROM tbRestaurante WHERE @range <= POINT(@lat @lng).STDistance(POINT(latRestaurante longRestaurante))", connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(restauranteSelectCommand))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            if(dt.Rows.Count != 0)
            {   
                List<Restaurante> restaurantes = new List<Restaurante>();
                foreach(DataRow r in dt.Rows)
                {
                    restaurantes.Add(new Restaurante((string)r["restauranteNome"], (int)r["restauranteID"], (decimal)r["restauranteLat"], (decimal)r["restauranteLong"]));
                }
                return restaurantes;
            }
            Response.StatusCode = 500;
            return null;
        }

        // GET: api/Restaurante/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet]
        public IEnumerable<Restaurante> Get()
        {
            DataTable dt = new DataTable();
            using (
             SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
            )
            {
                using (SqlCommand restauranteSelectCommand = new SqlCommand("SELECT * FROM tbRestaurante", connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(restauranteSelectCommand))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            if (dt.Rows.Count != 0)
            {
                List<Restaurante> restaurantes = new List<Restaurante>();
                foreach (DataRow r in dt.Rows)
                {
                    restaurantes.Add(new Restaurante((string)r["restauranteNome"], (int)r["restauranteID"], (decimal)r["restauranteLat"], (decimal)r["restauranteLong"]));
                }
                return restaurantes;
            }
            Response.StatusCode = 500;
            return null;
        }
    }
}
