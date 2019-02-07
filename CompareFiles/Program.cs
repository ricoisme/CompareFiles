using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareFiles
{
    static class Program
    {
        static void Main(string[] args)
        {
           CreateHostBuildAsync(args).GetAwaiter().GetResult();
           Console.WriteLine("All done.");
#if DEBUG
            
               Console.ReadLine();
#endif
        }

        private static Task CreateHostBuildAsync(string[] args)
        {
            var configName = "appsettings.json";
            var basePath = Directory.GetCurrentDirectory();
            if (!File.Exists(Path.Combine(basePath, configName)))
            {
                throw new ArgumentNullException($"{Path.Combine(basePath, configName)} does not exists ");
            }
            var configuration = new ConfigurationBuilder()
               .SetBasePath(basePath)
               .AddJsonFile(configName, optional: false, reloadOnChange: true)
               .Build();
            var serviceProvider = new ServiceCollection()
                .SetupServiceCollection(configuration.GetSection("FileConfig"))
                .BuildServiceProvider();
            var fileConfig = serviceProvider.GetService<IOptions<FileConfig>>().Value;
            
            fileConfig?.Files.ToList().ForEach(x =>
            {
                if(!File.Exists(x.SourceFileName))
                {
                    throw new ApplicationException($"{x.SourceFileName} did not exists");
                }
            });
            fileConfig?.Files.ToList()
                .ForEach( async x =>
                {
                    if(!File.Exists(x.TargetFileName))
                    {
                       await File.WriteAllTextAsync(x.TargetFileName, "", Encoding.UTF8)
                        .ConfigureAwait(false);
                       Console.WriteLine($"{x.TargetFileName} is created.");
                    }
                    if(File.Exists(x.SourceFileName) && File.Exists(x.TargetFileName))
                    {
                        var changeTotals = 0;
                        var sb = new StringBuilder();
                        using (var fs = new FileStream(x.SourceFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (var srcReader = new StreamReader(fs))
                            {
                                Console.WriteLine($"{x.SourceFileName} start reading..");
                                using (var tarReader = new StreamReader(x.TargetFileName))
                                {
                                    while (true)
                                    {
                                        if (srcReader.EndOfStream)
                                            break;
                                        var srcReaderTxt = srcReader.ReadLine();
                                        var tarReaderTxt = tarReader.ReadLine();
                                        if (!srcReaderTxt.Equals(tarReaderTxt))
                                        {
                                            sb.AppendLine(srcReaderTxt);
                                            changeTotals++;
                                        }
                                    }
                                }
                                Console.WriteLine($"{x.SourceFileName} read ended. different:{changeTotals}");
                            }
                        }
                        if (sb.ToString().Length > 0)
                        {
                            await File.AppendAllTextAsync(x.TargetFileName, sb.ToString(), Encoding.UTF8);
                            Console.WriteLine($"{x.TargetFileName} different content append done.");
                        }
                        else
                        {
                            Console.WriteLine($"{x.TargetFileName} no changed.");
                        }
                    }
                });
            return Task.FromResult(0);
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetupServiceCollection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<FileConfig>(configuration);
            services.AddLogging(configure =>
            {
                configure.AddDebug();
            });
            return services;
        }
    }
}
