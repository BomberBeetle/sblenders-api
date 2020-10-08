using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SblendersAPI.Models
{
    public class Pedido
    {
        public int restauranteID;
        public int agenteID;
        public int estadoID;
        public DateTime dataHoraPedido;
        public string endereco;
        public PedidoProduto[] produtos;
        public string instrucoes;

        public Pedido(int restauranteID, int agenteID, int estadoID, DateTime dataHoraPedido, string endereco, PedidoProduto[] produtos, string instrucoes)
        {
            this.restauranteID = restauranteID;
            this.agenteID = agenteID;
            this.estadoID = estadoID;
            this.dataHoraPedido = dataHoraPedido;
            this.endereco = endereco;
            this.produtos = produtos;
            this.instrucoes = instrucoes;
        }
    }
}
