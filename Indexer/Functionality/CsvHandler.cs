using Indexer.Common;
using Indexer.POCOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Indexer.Functionality
{
    public class CsvHandler
    {
        public void LoadResultsIntoDb()
        {
            Database database = new Database();
            DateOperations csv = new DateOperations();
            var reader = new StreamReader("D:\\Dropbox\\Share\\Excels\\Indexer Data\\scores.csv");
            List<string> records = new List<string>();
            bool isFirstLine = true;

            while (!reader.EndOfStream)
            {
                var values = reader.ReadLine().Split(';');

                if (isFirstLine == true)
                {
                    isFirstLine = false;
                    continue;
                }
              
                records.Add(values[0]);
            }

            foreach (var record in records)
            {
                List<string> values = record.Split(',').ToList();
                database.Results.Add(new Result()
                {
                    Date = csv.StringToInt(values[0]),
                    HomeTeam = values[1],
                    AwayTeam = values[2],
                    HomeGoals = int.Parse(values[3]),
                    AwayGoals = int.Parse(values[4]),
                    Competition = values[5],
                    Neutral = Convert.ToBoolean(values[8])
                });
            }

            database.SaveChanges();
        }
    }
}
