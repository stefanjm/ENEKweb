using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ENEKweb {
    public class Program {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        // Run specified startup config ( Development, Production, Default)
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            var assemblyName = typeof(Startup).GetTypeInfo().Assembly.FullName;

            // For this one use 
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup(assemblyName)
                .UseUrls("http://localhost:5001/");
        }
    }
}
