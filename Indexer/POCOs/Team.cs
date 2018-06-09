using System;

namespace Indexer.POCOs
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Confederation { get; set; }
        public double Index { get; set; }
        public int Version { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool Active { get; set; }
    }
}
