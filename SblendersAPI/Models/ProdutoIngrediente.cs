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
        public int PIngredientID;
        public int CategoriaIngredienteID;
        public decimal Price;
        public string Name;
        public string Desc;

        public ProdutoIngrediente(int defaultQuantity, int ingredientID, decimal price, string name, string desc, int pIngredientID, int cIngredientID)
        {
            PIngredientID = pIngredientID;
            DefaultQuantity = defaultQuantity;
            IngredientID = ingredientID;
            CategoriaIngredienteID = cIngredientID;
            Price = price;
            Name = name;
            Desc = desc;
        }
    }
}
