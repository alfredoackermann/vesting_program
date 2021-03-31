using System;

namespace VestingProgram.Domain
{
    public class Event
    {
        public EventType Type { get; private set; }
        public string EmployeeId { get; private set; }
        public string EmployeeName { get; private set; }
        public string AwardId { get; private set; }
        public DateTime Date { get; private set; }
        public Decimal Quantity { get; private set; }

        public Event(EventType type, string employeeId, string employeeName, string awardId, DateTime date, decimal quantity)
        {
            Type = type;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            AwardId = awardId;
            Date = date;
            Quantity = quantity;
        }
    }

    public enum EventType
    {
        VEST,
        CANCEL
    }
}
