using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VestingProgram.Domain;

namespace VestingProgram.Infrastructure
{
    public class CsvEventParser : ICsvEventParser
    {
        public async Task<List<Event>> ParseEventsAsync(string csvFilePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, config);
            var eventList = new List<Event>();
            var errorList = new List<string>();

            //Async read for better peformance
            while (await csv.ReadAsync())
            {
                try
                {
                    var eventType = csv.GetField<EventType>(0);

                    switch (eventType)
                    {
                        //Manual mapping required because cancellation events doesn't have the EMPLOYEE NAME column
                        case EventType.VEST:
                            eventList.Add(new Event(
                                eventType,
                                csv.GetField(1),
                                csv.GetField(2),
                                csv.GetField(3),
                                csv.GetField<DateTime>(4),
                                csv.GetField<decimal>(5)));
                            break;

                        case EventType.CANCEL:
                            eventList.Add(new Event(
                                eventType,
                                csv.GetField(1),
                                null,
                                csv.GetField(2),
                                csv.GetField<DateTime>(3),
                                csv.GetField<decimal>(4)));
                            break;
                    }
                }
                catch (Exception e)
                {
                    errorList.Add(string.Format("Error in row {0}: {1}", csv.CurrentIndex, e.Message));
                }
            }

            //If there is any error in the csv data, the program throws an exception detailing the errors found
            if (errorList.Any())
            {
                throw new FileFormatException(errorList.Aggregate((x, y) => x + Environment.NewLine + y));
            }

            return eventList;
        }
    }
}