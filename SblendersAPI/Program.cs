using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SblendersAPI
{
    public class Program
    {
        public static string dbPass = "gallelli";
        public static string dbLogin = "sa";
        public static string hashSalt = "зձʨC1ӂݹ事䀹ۢ׶8ޮҪؒ㨄瀸긡Ő箙荈�!ǰᅠ캅B4䞅뽃`oʰ씸戠־nAᷙꐼTHźX6Â6Ӽ◑NDkayCMms¾놽";
        public static string dbEnv = Environment.MachineName;
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
