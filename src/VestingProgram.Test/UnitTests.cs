using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VestingProgram.Application;
using VestingProgram.Domain;
using VestingProgram.Infrastructure;
using Xunit;

namespace VestingProgram.Test
{
    public class UnitTests
    {
        private readonly VestingService vestingService = new VestingService(new CsvEventParser(), new EventService());

        [Fact]
        public async Task TestCsvParseAsync()
        {
            var csvFileName = "example1.csv";

            if (File.Exists(csvFileName))
            {
                File.Delete(csvFileName);
            }

            using var writer = new StreamWriter(csvFileName, true);
            writer.WriteLine("VEST,E001,Alice Smith,ISO-001,2020-01-01,1000");
            writer.WriteLine("VEST,E001,Alice Smith,ISO-001,2021-01-01,1000");
            writer.WriteLine("VEST,E001,Alice Smith,ISO-002,2020-03-01,300");
            writer.WriteLine("VEST,E001,Alice Smith,ISO-002,2020-04-01,500");
            writer.WriteLine("VEST,E002,Bobby Jones,NSO-001,2020-01-02,100");
            writer.WriteLine("VEST,E002,Bobby Jones,NSO-001,2020-02-02,200");
            writer.WriteLine("VEST,E002,Bobby Jones,NSO-001,2020-03-02,300");
            writer.Close();

            var evtCount = (await new CsvEventParser().ParseEventsAsync(csvFileName)).Count;

            Assert.Equal(7, evtCount);
        }

        [Fact]
        public async Task TestExample1Async()
        {
            var csvFileName = "example1.csv";

            if (File.Exists(csvFileName))
            {
                File.Delete(csvFileName);
            }

            using var writer = new StreamWriter(csvFileName, true);
            writer.WriteLine("VEST,E001,Alice Smith,ISO-001,2020-01-01,1000");
            writer.WriteLine("VEST,E001,Alice Smith,ISO-001,2021-01-01,1000");
            writer.WriteLine("VEST,E001,Alice Smith,ISO-002,2020-03-01,300");
            writer.WriteLine("VEST,E001,Alice Smith,ISO-002,2020-04-01,500");
            writer.WriteLine("VEST,E002,Bobby Jones,NSO-001,2020-01-02,100");
            writer.WriteLine("VEST,E002,Bobby Jones,NSO-001,2020-02-02,200");
            writer.WriteLine("VEST,E002,Bobby Jones,NSO-001,2020-03-02,300");
            writer.Close();

            var result = await vestingService.GenerateVestingScheduleAsync(csvFileName, DateTime.Parse("2020-04-01"), 0);

            var expected = new List<(string, string, string, decimal)> {
                ("E001", "Alice Smith", "ISO-001", 1000),
                ("E001", "Alice Smith", "ISO-002", 800),
                ("E002", "Bobby Jones", "NSO-001", 600)
            };

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public async Task TestExample2Async()
        {
            var csvFileName = "example2.csv";

            if (File.Exists(csvFileName))
            {
                File.Delete(csvFileName);
            }

            using var writer = new StreamWriter(csvFileName, true);
            writer.WriteLine("VEST,E001,Alice Smith,ISO-001,2020-01-01,1000");
            writer.WriteLine("CANCEL,E001,ISO-001,2021-01-01,700");
            writer.Close();

            var result = await vestingService.GenerateVestingScheduleAsync(csvFileName, DateTime.Parse("2021-01-01"), 0);

            var expected = new List<(string, string, string, decimal)> {
                ("E001", "Alice Smith", "ISO-001", 300),
            };

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public async Task TestExample3Async()
        {
            var csvFileName = "example3.csv";

            if (File.Exists(csvFileName))
            {
                File.Delete(csvFileName);
            }

            using var writer = new StreamWriter(csvFileName, true);
            writer.WriteLine("VEST,E001,Alice Smith,ISO-001,2020-01-01,1000.5");
            writer.WriteLine("CANCEL,E001,ISO-001,2021-01-01,700.75");
            writer.Close();

            var result = await vestingService.GenerateVestingScheduleAsync(csvFileName, DateTime.Parse("2021-01-01"), 1);

            var expected = new List<(string, string, string, decimal)> {
                ("E001", "Alice Smith", "ISO-001", (decimal)299.8),
            };

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
        }
    }
}
