using Indexer.POCOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer.Functionality
{
    public class ConfederationHandler
    {
        Database database = new Database();
        ResultsHandler resultsHandler = new ResultsHandler();
        TeamHandler teamHandler = new TeamHandler();

        public double GetIndex(string name)
        {
            return database.Confederations.Where(x => x.Name == name && x.Active == true).FirstOrDefault().Index;
        }

        public string GetConfederationFromTxt(string teamName)
        {
            StreamReader file = new StreamReader("D:\\Dropbox\\Share\\Excels\\Indexer Data\\teams.txt");
            string confederation = "";

            while (!file.EndOfStream)
            {
                var line = file.ReadLine().Split('=');
                if (line[1] == teamName) confederation = line[0];
            }

            file.Close();
            return confederation;
        }

        public void UpdateConfederations(List<int> dates)
        {
            List<Result> results = resultsHandler.GetWorldCupResults(dates);
            List<string> teams = teamHandler.GetAllUniqueTeams(results);
            Dictionary<string, double> indexMultipliers = teamHandler.CalculateIndexChanges(dates, results, teams);
            Dictionary<string, double> confMultipliers = CalculateConfIndexChange(indexMultipliers, dates.Last());
            List<Confederation> confederations = database.Confederations.Where(x => x.Active == true).ToList();
            teamHandler.UpdateIndex(teams, confMultipliers, dates.Last(), confederations);
        }

        public Dictionary<string, double> CalculateConfIndexChange(Dictionary<string, double> indexMultipliers, int date)
        {
            List<Team> teams = teamHandler.GetTeamsByName(indexMultipliers.Keys);
            List<Confederation> confederations = database.Confederations.Where(x => x.Active == true).ToList();
            Dictionary<string, double> confederationMultipliers = new Dictionary<string, double>();

            foreach (var confederation in confederations)
            {
                List<Team> members = teams.Where(x => x.Confederation == confederation.Name).ToList();
                double confederationMultiplier;
                Confederation newVersion = new Confederation()
                {
                    Name = confederation.Name,
                    Active = true,
                    LastUpdated = date,
                    Version = confederation.Version++
                };

                if (members == null)
                {
                    confederation.Active = false;                    
                    newVersion.Index = confederation.Index > 50 ? 50 : confederation.Index;
                    confederationMultiplier = newVersion.Index / confederation.Index;
                    confederationMultipliers.Add(confederation.Name, confederationMultiplier);
                    database.Confederations.Add(newVersion);
                    database.SaveChanges();    
                    continue;
                }

                List<double> indexeChanges = new List<double>();

                foreach (var member in members)
                {
                    indexeChanges.Add(indexMultipliers[member.Name]);
                }

                confederationMultiplier = indexeChanges.Average();
                newVersion.Index = Math.Round(confederation.Index * confederationMultiplier, 3);
                confederation.Active = false;
                confederationMultipliers.Add(confederation.Name, confederationMultiplier);
                database.Confederations.Add(newVersion);
                database.SaveChanges();
            }

            return confederationMultipliers;
        }
    }
}
