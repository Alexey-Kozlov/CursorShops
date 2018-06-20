using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CursorShops
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
               // .UseKestrel(options=> { options.Limits.MaxRequestBodySize = 1024 * 1024; })
                .UseIISIntegration()
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .Build();
    }
}
