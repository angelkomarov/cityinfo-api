using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace CityInfoAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                //Run the host and block the calling thread until the Host shuts down
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Error in init");
                throw;
            }
            finally
            {
                //NLog.LogManager.Shutdown();
            }
        }
      
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            //WebHost.CreateDefaultBuilder(args)
            //    .UseStartup<Startup>();

            //adding NLog provider service.
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .UseNLog();
    }
}
