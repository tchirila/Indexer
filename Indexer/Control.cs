using Indexer.Functionality;
using Indexer.POCOs;
using System.Collections.Generic;
using System.Linq;

namespace Indexer
{
    public class Control
    {
        CsvHandler csvHandler = new CsvHandler();
        ResultsHandler resultsHanlder = new ResultsHandler();
        Database database = new Database();

        public void Run()
        {
            Status status = database.Status.ToList().FirstOrDefault();
            List<ConfederationChange> changes = database.ConfederationChanges.OrderBy(x => x.Date).ToList();

            if (status.Date == 0) csvHandler.LoadResultsIntoDb();

            foreach (var change in changes)
            {
                resultsHanlder.Run(status, change.Date);              
            }

            resultsHanlder.Run(status, 99991228);
        }
    }
}
