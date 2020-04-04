using SblendersAPI.Utils;
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

        public ClienteOnline(string Nome, string Sobrenome, string Login, string Password)
        {
            this.Nome = Nome;
            this.Sobrenome = Sobrenome;
            this.Login = Login;
            this.Password = Password;
        }
    }
}
