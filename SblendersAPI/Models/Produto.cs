using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SblendersAPI.Models
{
    public class Produto : ProdutoParcial
{
        public string Desc;
        public ProdutoIngrediente[] ingredientes;

        public Produto(int iD, decimal cost, string name, string desc, ProdutoIngrediente[] ingredientes) : base(iD, cost, name)
        {
            Desc = desc;
            this.ingredientes = ingredientes;
        }
    }
}
