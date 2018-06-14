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
        TeamHandler teamHandler = new TeamHandler();        

        public void UpdateConfederations(List<int> dates)
        {
            List<Result> results = GetWorldCupResults(dates);
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
                double confederationMultiplier = 1;
                Confederation newVersion = new Confederation()
                {
                    Name = confederation.Name,
                    Active = true,
                    LastUpdated = date,
                    Version = confederation.Version++
                };

                if (members.Count == 0)
                {
                    confederation.Active = false;
                    newVersion.Index = confederation.Index > 50 ? 50 : confederation.Index;
                    confederationMultiplier = newVersion.Index / confederation.Index;
                    confederationMultipliers.Add(confederation.Name, confederationMultiplier);
                    database.Confederations.Add(newVersion);
                    database.SaveChanges();
                }
                else if (members.Count > 0)
                {
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
            }

            return confederationMultipliers;
        }
        public List<Result> GetWorldCupResults(List<int> dates)
        {
            int startDate = dates.First();
            int endDate = dates.Last();
            return database.Results.Where(x => x.Competition == "FIFA World Cup" && x.Date >= startDate && x.Date <= endDate).ToList();
        }
    }
}
