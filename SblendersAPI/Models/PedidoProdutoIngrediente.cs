using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SblendersAPI.Models
{
    public class PedidoProdutoIngrediente
    {
        public int IngredientID;
        public int Quantidade;

        public PedidoProdutoIngrediente(int ingredientID, int quantidade)
        {
            IngredientID = ingredientID;
            Quantidade = quantidade;
        }
    }
}
