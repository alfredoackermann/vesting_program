using System;
using System.Collections.Generic;
using System.Linq;

namespace VestingProgram.Domain
{
    public class EventService : IEventService
    {
        

        public IEnumerable<(string EmployeeId, string EmployeeName, string AwardId, decimal Quantity)> CalculateVestSchedule(List<Event> eventList, DateTime targetDate, int decimalPrecision) =>
            //Group events by EMPLOYEE ID and AWARD ID for the vest calculations
            eventList.GroupBy(evt => (evt.EmployeeId, evt.AwardId))
                     .Select(evtGroup => (evtGroup.Key.EmployeeId,
                                          evtGroup.First(evt => evt.Type == EventType.VEST).EmployeeName, //Required because cancellation events doesn't have the EMPLOYEE NAME column
                                          evtGroup.Key.AwardId,
                                          CalculateVestedShares(evtGroup.Select(evt => evt),
                                                                targetDate,
                                                                decimalPrecision)));

        private decimal CalculateVestedShares(IEnumerable<Event> events, DateTime targetDate, int decimalPrecision)
        {
            var dateFilterQuery = events.Where(evt => evt.Date <= targetDate);

            //Include all employees and awards in the output, even if no shares are
            //vested by the given date
            if (!dateFilterQuery.Any())
            {
                return 0;
            }

            var vestEventQuery = dateFilterQuery.Where(evt => evt.Type == EventType.VEST);
            var cancelEventQuery = dateFilterQuery.Where(evt => evt.Type == EventType.CANCEL);

            var totalVest = CalculateTotalShares(vestEventQuery, decimalPrecision);
            var totalCancel = CalculateTotalShares(cancelEventQuery, decimalPrecision);

            //Subtract all shares cancelled
            var result = totalVest - totalCancel;

            //If the sum of canceled shares exceeds the number of shares vested, then the
            //output should show zero shares vested.
            result = result >= 0 ? result : 0;

            return result;
        }

        //Calculate the total number of shares per event type. 
        //Round down the inputs to the specified precision.  
        private decimal CalculateTotalShares(IEnumerable<Event> events, int decimalPrecision) => events.Any() ? events.Sum(evt => decimal.Round(evt.Quantity, decimalPrecision, MidpointRounding.ToZero)) : 0;

    }
}
