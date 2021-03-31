using System.Collections.Generic;
using System.Threading.Tasks;

namespace VestingProgram.Domain
{
    public interface ICsvEventParser
    {
        Task<List<Event>> ParseEventsAsync(string csvFilePath);
    }
}
