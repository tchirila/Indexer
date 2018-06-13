using System.Collections.Generic;
using System.Linq;

namespace Indexer.Common
{
    public class DateOperations
    {
        public int StringToInt(string date)
        {
            List<string> values = date.Split('/').ToList();
            date = values[2] + values[1] + values[0];

            return int.Parse(date);
        }
    }
}
