using System;
using System.Collections.Generic;
using System.Linq;

namespace VestingProgram.Domain
{
    public interface IEventService
    {
        IEnumerable<(string EmployeeId, string EmployeeName, string AwardId, decimal Quantity)> CalculateVestSchedule(List<Event> eventList, DateTime targetDate, int decimalPrecision);
    }
}
