using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SblendersAPI.Models
{
    public class PedidoProdutoIngrediente
    {
        public int ProdutoIngredienteID;
        public int Quantidade;
        public int PedidoProdutoIngredienteID;
        

        public PedidoProdutoIngrediente(int ingredientID, int quantidade, int ppiID)
        {
            ProdutoIngredienteID = ingredientID;
            Quantidade = quantidade;
            PedidoProdutoIngredienteID = ppiID;
        }
    }
}
