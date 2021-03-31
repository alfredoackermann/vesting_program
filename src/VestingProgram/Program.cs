using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using VestingProgram.Application;
using VestingProgram.Domain;
using VestingProgram.Infrastructure;

namespace VestingProgram
{
    class Program
    {   
        static async Task Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");

            using IHost host = CreateHostBuilder(args).Build();

            try
            {
                ValidateArguments(args);

                var csvFilePath = args[0];
                var targetDate = DateTime.Parse(args[1]);
                int decimalPrecision = 0;

                if (args.Length == 3)
                {
                    decimalPrecision = int.Parse(args[2]);
                }

                var vestingService = host.Services.GetService<IVestingService>();

                var vestingSchedule = await vestingService.GenerateVestingScheduleAsync(csvFilePath, targetDate, decimalPrecision);

                foreach (var item in vestingSchedule)
                {
                    Console.WriteLine(item.Item1 + "," + item.Item2 + "," + item.Item3 + "," + item.Item4);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private static void ValidateArguments(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Invalid number of arguments");
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddScoped<IVestingService, VestingService>()
                            .AddScoped<ICsvEventParser, CsvEventParser>()
                            .AddScoped<IEventService, EventService>());
    }
}
