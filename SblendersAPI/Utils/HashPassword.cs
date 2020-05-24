using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace SblendersAPI.Utils
{

    class PasswordHasher
    {
        public static string Hash(string pass, string salt)
        {
            Rfc2898DeriveBytes pbkdf = new Rfc2898DeriveBytes(pass, Encoding.UTF8.GetBytes(salt), 100);
            return BitConverter.ToString(pbkdf.GetBytes(32)).Replace("-", "");
        }
    }

}
