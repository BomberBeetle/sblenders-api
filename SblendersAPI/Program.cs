﻿using System;
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
        public static string dbPass = "12345";
        //public static string dbPass = "Abc12345";
        //public static string dbPass = "gallelli";
        public static string dbLogin = "sa";
        public static string dbEnv = Environment.MachineName;
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().UseKestrel();
            
    }
}
