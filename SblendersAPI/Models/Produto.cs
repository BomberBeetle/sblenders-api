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
        public InformacaoNutricional[] infoNutr;

        public Produto(int iD, decimal cost, string name, string desc, ProdutoIngrediente[] ingredientes, InformacaoNutricional[] infoNutr) : base(iD, cost, name)
        {
            Desc = desc;
            this.ingredientes = ingredientes;
            this.infoNutr = infoNutr;
        }

    public class InformacaoNutricional{
        public string descricao;
        public int val;

        public InformacaoNutricional(string descricao, int val){
            this.val = val;
            this.descricao = descricao;
        }
    }
    }
}
