using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SblendersAPI.Models
{
    public class ClienteOnline:Agente
    {
        public string Nome;
        public string Sobrenome;
        new public readonly int TipoAgenteID = 1;
    }
}
