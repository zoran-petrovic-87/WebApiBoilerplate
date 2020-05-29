using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace WebApi
{
    /// <summary>
    /// Contains the method <c>Main</c>, which is the entry point of the application.
    /// The main purpose of the this class is to configure the applications infrastructure.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            // Configure NLog depending on environment.
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            NLogBuilder.ConfigureNLog(env == Environments.Production ? "NLog.config" : "NLog.Development.config");
            // Uncomment next line if you experience issues with NLog and you need to debug.
            // LogManager.ThrowExceptions = true;

            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>An object that encapsulates all of the applications resources, such as:
        /// <list type="bullet">
        ///   <item>An HTTP server implementation</item>
        ///   <item>Middleware components</item>
        ///   <item>Logging</item>
        ///   <item>DI</item>
        ///   <item>Configuration</item>
        /// </list>
        /// </returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>().UseNLog(); });
    }
}