using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SblendersAPI.Models
{
    public class PedidoProduto
    {
        public int pedidoProdutoID;
        public int pedidoProdutoQtde;
        public int produtoID;
        public PedidoProdutoIngrediente[] ingredientes;

        public PedidoProduto(int pedidoProdutoQtde, int produtoID, PedidoProdutoIngrediente[] ingredientes)
        {
            this.pedidoProdutoQtde = pedidoProdutoQtde;
            this.produtoID = produtoID;
            this.ingredientes = ingredientes;
        }
    }
}
