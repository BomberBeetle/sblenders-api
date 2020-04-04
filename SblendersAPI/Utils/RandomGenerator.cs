using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SblendersAPI.Utils
{
    public static class RandomGenerator
    {
        public static string GenerateHexString(int count)
        {
            Random r = new Random();
            byte[] bytes = new byte[count];
            r.NextBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
