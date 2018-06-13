namespace Indexer.POCOs
{
    public class Result
    {
        public int Id { get; set; }
        public int Date { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public string Competition { get; set; }
        public int ExpectedDifference { get; set; }
        public bool Neutral { get; set; }  
    }
}
