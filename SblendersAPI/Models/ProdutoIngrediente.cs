using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SblendersAPI.Models
{
    public class ProdutoIngrediente
    {
        public int DefaultQuantity;
        public int IngredientID;
        public decimal Price;
        public string Name;
        public string Desc;

        public ProdutoIngrediente(int defaultQuantity, int ingredientID, decimal price, string name, string desc)
        {
            DefaultQuantity = defaultQuantity;
            IngredientID = ingredientID;
            Price = price;
            Name = name;
            Desc = desc;
        }
    }
}
