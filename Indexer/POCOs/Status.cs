using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer.POCOs
{
    public class Status
    {
        public int Id { get; set; }
        public int Date { get; set; }
        public string State { get; set; }
        public int TotalGoalDifference { get; set; }
        public int TotalMatchesPlayed { get; set; }
    }
}
