using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VestingProgram.Domain;

namespace VestingProgram.Application
{
    public class VestingService : IVestingService
    {
        private readonly ICsvEventParser _csvParser;
        private readonly IEventService _eventService;

        public VestingService(ICsvEventParser csvParser, IEventService eventService)
        {
            _csvParser = csvParser;
            _eventService = eventService;
        }

        public async Task<IEnumerable<(string, string, string, decimal)>> GenerateVestingScheduleAsync(string csvFilePath, DateTime targetDate, int decimalPrecision)
        {
            ValidateParameters(csvFilePath, decimalPrecision);

            var eventList = await _csvParser.ParseEventsAsync(csvFilePath);
            var vestSchedule = _eventService.CalculateVestSchedule(eventList, targetDate, decimalPrecision);

            //Output should be ordered by Employee ID and Award ID
            return vestSchedule.OrderBy(vest => (vest.EmployeeId, vest.AwardId)).AsEnumerable();
        }
        
        private void ValidateParameters(string csvFilePath, int decimalPrecision)
        {
            if (!File.Exists(csvFilePath))
            {
                throw new FileNotFoundException(null, csvFilePath);
            }

            if (decimalPrecision < 0 || decimalPrecision > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPrecision), decimalPrecision, "Valid values are between 0 (default) and 6.");
            }
        }
    }
}
