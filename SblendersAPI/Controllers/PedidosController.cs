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
        [HttpGet("{agent_id}", Name = "IndexPedidos")]
        public IEnumerable<Dictionary<string, string>> Get(int agent_id)
        {
            using (
              SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
              )
            {
                int agentType;
                int? restauranteID;
                using (
                    SqlCommand agenteQueryCommand = new SqlCommand("SELECT tipoAgenteID, restauranteID FROM tbAgente LEFT OUTER JOIN tbClienteOnline ON tbClienteOnline.agenteID = tbAgente.agenteID LEFT OUTER JOIN tbFuncionario ON tbFuncionario.agenteID = tbAgente.agenteID WHERE agenteToken = @token AND tbAgente.agenteID = @id ;", connection)
                )
                {
                    agenteQueryCommand.Parameters.Add(new SqlParameter("@id", agent_id));
                    agenteQueryCommand.Parameters.Add(new SqlParameter("@token", Request.Headers["Authorization"].ToString()));
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(agenteQueryCommand))
                    {
                        DataTable t = new DataTable();
                        adapter.Fill(t);
                        if (t.Rows.Count < 1)
                        {
                            Response.StatusCode = StatusCodes.Status403Forbidden;
                            return new Dictionary<string, string>[0];
                        }
                        agentType = (int)t.Rows[0]["tipoAgenteID"];
                        restauranteID = t.Rows[0]["restauranteID"]!=DBNull.Value?(int)t.Rows[0]["restauranteID"]:0;
                    }
                }
                if (agentType == 1)
                {
                    //ClienteOnline
                    using (
                    SqlCommand pedidosQueryCommand = new SqlCommand("SELECT pedidoID, pedidoDataHora FROM tbPedido WHERE agenteID = @id", connection)
                    )
                    {
                        pedidosQueryCommand.Parameters.Add(new SqlParameter("@id", agent_id));
                        DataTable data = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(pedidosQueryCommand))
                        {
                            adapter.Fill(data);
                        }
                        Dictionary<string, string>[] queryReturn = new Dictionary<string, string>[data.Rows.Count];
                        for (int i = 0; i < data.Rows.Count; i++)
                        {
                            queryReturn[i] = new Dictionary<string, string> {
                                {"pedidoID", data.Rows[i]["pedidoID"].ToString() },
                                {"pedidoDataHora",data.Rows[i]["pedidoID"].ToString()}
                            };
                        }
                        return queryReturn;
                    }
                }
                else if (agentType == 2)
                {
                    using (
                    SqlCommand pedidosQueryCommand = new SqlCommand("SELECT pedidoID, pedidoDataHora FROM tbPedido WHERE restauranteID = @id AND estadoPedidoID IN (0,1,2,3)", connection)
                    )
                    {
                        
                        pedidosQueryCommand.Parameters.Add(new SqlParameter("@id", restauranteID));
                        DataTable data = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(pedidosQueryCommand))
                        {
                            adapter.Fill(data);
                        }
                        Dictionary<string, string>[] queryReturn = new Dictionary<string, string>[data.Rows.Count];
                        for (int i = 0; i < data.Rows.Count; i++)
                        {
                            queryReturn[i] = new Dictionary<string, string> {
                                {"pedidoID", data.Rows[i]["pedidoID"].ToString() },
                                {"pedidoDataHora",data.Rows[i]["pedidoID"].ToString()}
                            };
                        }
                        return queryReturn;
                    }
                }
            }
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new Dictionary<string, string>[0];
        }

        /*
        // GET: api/Pedidos/5
        [HttpGet("{agent_id}/{id}", Name = "Get")]
        public Pedido Get(int agent_id, int id)
        {
            return "value";
        }
        */

        // POST: api/Pedidos
        [HttpPost("{agenteID}/{pedidoID}/{estadoID}")]
        public void Post(int agenteID, int pedidoID)
        {
            using (
            SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
            )
            {
                connection.Open();
                using (
                    SqlCommand agenteQueryCommand = new SqlCommand("SELECT tipoAgenteID FROM tbAgente WHERE agenteToken = @token AND agenteID = @id ;", connection)
                )
                {
                    int? tipoAgenteID = (int?)agenteQueryCommand.ExecuteScalar();
                    if(tipoAgenteID != 2 || tipoAgenteID == null)
                    {
                        Response.StatusCode = StatusCodes.Status403Forbidden;
                        return;
                    }
                }
                using (
                    SqlCommand updatePedidoCommand = new SqlCommand("UPDATE tbPedido SET estadoPedidoID = @estadoID WHERE pedidoID=@pedidoID", connection)
                    )
                {
                    if(updatePedidoCommand.ExecuteNonQuery() < 1)
                    {
                        Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }
            }
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

        private void RevertPedido(int pedidoID)
        {

        }
    }
}
