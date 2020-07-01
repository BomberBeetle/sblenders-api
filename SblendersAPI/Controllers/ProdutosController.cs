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
        public IEnumerable<ProdutoParcial> Get()
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
                SqlCommand produtosQueryCommand = new SqlCommand($"SELECT produtoID, produtoNome, produtoCusto FROM tbProduto WHERE {(filterByCategory ? "categoriaID = @cat" : "1=1")} AND {(filterByCategory ? $"produtoNome LIKE '%@query%'" : "1=1")} ORDER BY {sortString} OFFSET @offset ROWS FETCH NEXT @itemCount ROWS ONLY;", connection)
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
                    List<ProdutoParcial> produtos = new List<ProdutoParcial>();
                    foreach(DataRow r in dataTable.Rows)
                    {
                        var p = new ProdutoParcial((int)r["produtoID"], (decimal)r["produtoCusto"], (string)r["produtoNome"]);
                        produtos.Add(p);
                    }
                    return produtos;
                }
            }
        }

        // GET: api/Produtos/5
        [HttpGet("{id}", Name = "GetProduto")]
        public Produto Get(int id)
        {
            using (
              SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
            )
            using (
                SqlCommand produtoQueryCommand = new SqlCommand("SELECT produtoID, produtoNome, produtoCusto, produtoDescricao FROM tbProduto WHERE produtoID = @id", connection)
            )
            {
                produtoQueryCommand.Parameters.Add(new SqlParameter("@id", id));
                using (SqlDataAdapter productAdapter = new SqlDataAdapter(produtoQueryCommand))
                {
                    DataTable product = new DataTable();
                    connection.Open();
                    productAdapter.Fill(product);
                    if (product.Rows.Count < 1)
                    {
                        Response.StatusCode = StatusCodes.Status400BadRequest;
                        return null;
                    }
                    else
                    {
                        using (
                        SqlCommand produtoIngQueryCommand = new SqlCommand("SELECT tbIngrediente.ingredienteID, ingredienteNome, produtoIngredienteID, ingredienteCusto, novoPreco, quantidadePadrao, ingredienteDescricao, categoriaIngredienteID FROM tbProdutoIngrediente INNER JOIN tbIngrediente ON tbIngrediente.ingredienteID = tbProdutoIngrediente.ingredienteID WHERE tbProdutoIngrediente.produtoID = @id", connection)
                        )
                        {
                            produtoIngQueryCommand.Parameters.Add(new SqlParameter("@id", id));
                            DataTable ingredients = new DataTable();
                            using (SqlDataAdapter ingredientsAdapter = new SqlDataAdapter(produtoIngQueryCommand))
                            {
                                ingredientsAdapter.Fill(ingredients);
                                List<ProdutoIngrediente> ingList = new List<ProdutoIngrediente>();
                                foreach(DataRow r in ingredients.Rows)
                                {
                                    decimal preco;
                                    if(r["novoPreco"] == DBNull.Value)
                                    {
                                        preco = (decimal)r["ingredienteCusto"];
                                    }
                                    else
                                    {
                                        preco = (decimal)r["novoPreco"];
                                    }
                                    ProdutoIngrediente ingrediente = new ProdutoIngrediente((int)r["quantidadePadrao"], (int)r["ingredienteID"], preco, (string)r["ingredienteNome"], (string)r["ingredienteDescricao"], (int)r["produtoIngredienteID"], (int)r["categoriaIngredienteID"]);
                                    ingList.Add(ingrediente);
                                }
                                Produto produto = new Produto(id, (decimal)product.Rows[0]["produtoCusto"], product.Rows[0]["produtoNome"].ToString(), product.Rows[0]["produtoDescricao"].ToString(),ingList.ToArray());
                                return produto;
                            }
                        }
                    }
                }
            }
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
