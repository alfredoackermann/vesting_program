using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VestingProgram.Application
{
    public interface IVestingService
    {
        Task<IEnumerable<(string, string, string, decimal)>> GenerateVestingScheduleAsync(string csvFilePath, DateTime targetDate, int decimalPrecision);
    }
}
