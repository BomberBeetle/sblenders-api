using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SblendersAPI.Models
{
    public class Produto
    {
        public int ID;
        public decimal Cost;
        public string Name;
        public string Desc;
        public ProdutoIngrediente[] ingredientes;

        public Produto(int iD, decimal cost, string name, string desc, ProdutoIngrediente[] ingredientes)
        {
            ID = iD;
            Cost = cost;
            Name = name;
            Desc = desc;
            this.ingredientes = ingredientes;
        }
    }
}
