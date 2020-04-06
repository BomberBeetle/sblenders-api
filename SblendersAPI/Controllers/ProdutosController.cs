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
    public class ProdutosController : ControllerBase
    {
        // GET: api/Produtos
        [HttpGet]
        public IEnumerable<Produto> Get()
        {
            int page = 1;
            int itemCount = 20;
            bool filterByCategory = false;
            bool filterByQuery = false;
            int category = 0;
            string query = "";
            string sortString = "produtoID DESC";

            if (Request.Query["page"].Count != 0)
            {
                try
                {
                    page = int.Parse(Request.Query["page"][0]);
                }
                catch
                {

                }
            }
            if (Request.Query["itemCount"].Count != 0)
            {
                try
                {
                    itemCount = int.Parse(Request.Query["itemCount"][0]);
                }
                catch
                {

                }
            }
            if (Request.Query["query"].Count != 0)
            {
                try
                {
                    query = Request.Query["page"][0];
                }
                catch
                {

                }
            }
            if (Request.Query["category"].Count != 0)
            {
                try
                {
                    category = int.Parse(Request.Query["cat"][0]);
                    filterByCategory = true;
                }
                catch
                {
                    filterByCategory = false;
                }
            }
            if (Request.Query["sort"].Count != 0)
            {
               if(Request.Query["sort"][0] == "up")
                {
                     sortString = "produtoCusto ASC";
                }
               else if(Request.Query["sort"][0] == "dn")
                {
                    sortString = "produtoCusto DESC";
                }
            }
            using (
              SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
              )
            using (
                SqlCommand produtosQueryCommand = new SqlCommand($"SELECT produtoID, produtoNome, produtoCusto FROM tbPedido WHERE {(filterByCategory ? "categoriaID = @cat" : "1=1")} AND {(filterByCategory ? $"produtoNome LIKE '%@query%'" : "1=1")} ORDER BY {sortString} OFFSET @offset ROWS FETCH NEXT @itemCount ROWS ONLY;", connection)
            )
            {
                produtosQueryCommand.Parameters.Add(new SqlParameter("@cat", category));
                produtosQueryCommand.Parameters.Add(new SqlParameter("@query", query));
                produtosQueryCommand.Parameters.Add(new SqlParameter("@offset", (page - 1) * itemCount));
                produtosQueryCommand.Parameters.Add(new SqlParameter("@itemCount", itemCount));
                using (SqlDataAdapter adapter = new SqlDataAdapter(produtosQueryCommand))
                {
                    connection.Open();
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    List<Produto> produtos = new List<Produto>();
                    foreach(DataRow r in dataTable.Rows)
                    {
                        var p = new Produto((int)r["produtoID"], (decimal)r["produtoNome"], (string)r["produtoCusto"]);
                        produtos.Add(p);
                    }
                    return produtos;
                }
            }
        }

        // GET: api/Produtos/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Produtos
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Produtos/5
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
