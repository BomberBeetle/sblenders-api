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
                using (SqlCommand restauranteSelectCommand = new SqlCommand("SELECT * FROM tbRestaurante WHERE @range >= geography::Point(@lat, @lng, 4326).STDistance(geography::Point(restauranteLat, restauranteLong, 4326))", connection))
                {
                    restauranteSelectCommand.Parameters.Add(new SqlParameter("@range", range));
                    restauranteSelectCommand.Parameters.Add(new SqlParameter("@lat", lat ));
                    restauranteSelectCommand.Parameters.Add(new SqlParameter("@lng", lng));

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
            return new List<Restaurante>(); ;
        }

        // GET: api/Restaurante/5
        [HttpGet("{id}", Name = "GetRestaurante")]
        public Restaurante Get(int id)
        {
            DataTable dt = new DataTable();
            using (
             SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
            )
            {
                using (SqlCommand restauranteSelectCommand = new SqlCommand("SELECT * FROM tbRestaurante WHERE restauranteID = @id", connection))
                {
                    restauranteSelectCommand.Parameters.Add(new SqlParameter("@id", id));
                    using (SqlDataAdapter adapter = new SqlDataAdapter(restauranteSelectCommand))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            if (dt.Rows.Count != 0)
            {
                DataRow r = dt.Rows[0];

                return new Restaurante((string)r["restauranteNome"], (int)r["restauranteID"], (decimal)r["restauranteLat"], (decimal)r["restauranteLong"]);
            }

            Response.StatusCode = 404;
            return null;
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
