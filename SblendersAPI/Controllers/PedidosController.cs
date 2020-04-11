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
    public class PedidosController : ControllerBase
    {
        // GET: api/Pedidos
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Pedidos/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Pedidos
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Pedidos/5
        [HttpPut("{id}")]
        public void Put([FromBody] Pedido pedido)
        {
            DataTable agentData = new DataTable();
            int restaurantID = 0;
            int? pedidoID = null;
            using (
              SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
              )
            {
                
                using (
                    SqlCommand agenteQueryCommand = new SqlCommand("SELECT tipoAgenteID FROM tbAgente WHERE agenteToken = @token AND agenteID = @id ;", connection)
                )
                {
                    agenteQueryCommand.Parameters.Add(new SqlParameter("@id", pedido.agenteID));
                    agenteQueryCommand.Parameters.Add(new SqlParameter("@token", Request.Headers["Authorization"].ToString()));
                    connection.Open();
                    int? agentType = (int?)agenteQueryCommand.ExecuteScalar();
                    if(agentType == null)
                    {
                        Response.StatusCode = StatusCodes.Status403Forbidden;
                        return;
                    }
                }
                
                    using (
                        SqlCommand insertPedidoCommand = new SqlCommand("INSERT INTO tbPedido(restauranteID, agenteID, enderecoEntrega, dataHoraPedido, estadoPedidoID) VALUES(@restID, @agID, @endereco, @datahora, 0) SELECT CAST(SCOPE_IDENTITY() AS INT)", connection)
                    )
                    {
                        insertPedidoCommand.Parameters.Add(new SqlParameter("@restID", pedido.restauranteID));
                        insertPedidoCommand.Parameters.Add(new SqlParameter("@agID", pedido.agenteID));
                        insertPedidoCommand.Parameters.Add(new SqlParameter("@endereco", pedido.endereco));
                        insertPedidoCommand.Parameters.Add(new SqlParameter("@datahora", pedido.dataHoraPedido));
                        pedidoID = (int?)insertPedidoCommand.ExecuteScalar();
                        if(pedidoID != 1) {
                            Response.StatusCode = StatusCodes.Status500InternalServerError;
                            return;
                        }
                    }
                try
                {
                    foreach (PedidoProduto produto in pedido.produtos)
                    {
                        int pedProdID;
                        using (
                            SqlCommand insertProdutoCommand = new SqlCommand("INSERT INTO tbPedidoProduto(pedidoID, produtoID, pedidoProdutoQtde) VALUES(@pedID, @prodID, @qtde) SELECT CAST(SCOPE_IDENTITY() AS INT)", connection)
                        )
                        {
                            insertProdutoCommand.Parameters.Add(new SqlParameter("@pedID", pedidoID));
                            insertProdutoCommand.Parameters.Add(new SqlParameter("@prodID", produto.produtoID));
                            insertProdutoCommand.Parameters.Add(new SqlParameter("@qtde", produto.pedidoProdutoQtde));
                             pedProdID = (int)insertProdutoCommand.ExecuteScalar();
                        }
                        foreach (PedidoProdutoIngrediente ingrediente in produto.ingredientes)
                        {
                            using (
                                SqlCommand insertIngredienteCommand = new SqlCommand("INSERT INTO tbPedidoProdutoIngrediente(pedidoProdutoID, produtoIngredienteID, quantidadeIngrediente) VALUES(@ppID, @piID, @qtde) SELECT CAST(SCOPE_IDENTITY() AS INT)", connection)
                            )
                            {
                                insertIngredienteCommand.Parameters.Add(new SqlParameter("@ppID", pedProdID));
                                insertIngredienteCommand.Parameters.Add(new SqlParameter("@piID",ingrediente.ProdutoIngredienteID));
                                insertIngredienteCommand.Parameters.Add(new SqlParameter("@ppID", ingrediente.Quantidade));
                                insertIngredienteCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch
                {
                    RevertPedido(pedidoID.Value);
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private void RevertPedido(int pedidoID)
        {

        }
    }
}
