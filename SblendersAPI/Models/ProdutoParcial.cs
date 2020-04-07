using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SblendersAPI.Models
{
    public class ProdutoParcial
    {
        public int ID;
        public decimal Cost;
        public string Name;
        
        public ProdutoParcial(int ID, decimal Cost, string Name)
        {
            this.ID = ID;
            this.Cost = Cost;
            this.Name = Name;
        }
    }
}
