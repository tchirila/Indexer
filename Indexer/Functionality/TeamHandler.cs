using Indexer.POCOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer.Functionality
{
    public class TeamHandler
    {
        Database database = new Database();                       
        public void UpdateIndex(List<string> teams, Dictionary<string, double> confMultipliers, int date, List<Confederation> confederations)
        {
            foreach (var confederation in confederations)
            {
                List<Team> members = database.Teams.Where(x => x.Confederation == confederation.Name && !teams.Contains(x.Name) && x.Active == true).ToList();
                List<Team> updateMembers = new List<Team>();

                foreach (var member in members)
                {
                    member.Active = false;
                    updateMembers.Add(new Team
                    {
                        Name = member.Name,
                        Version = member.Version++,
                        LastUpdated = date,
                        Confederation = member.Confederation,
                        Active = true,
                        Idx = member.Idx * confMultipliers[confederation.Name]
                    });
                }

                database.SaveChanges();
            }
        }

        public List<Team> GetTeamsByName(Dictionary<string, double>.KeyCollection keys)
        {
            return database.Teams.Where(x => keys.Contains(x.Name) && x.Active == true).ToList();
        }

        public void WriteTeamsToTxt(Dictionary<string, int> teams)
        {
            List<string> list = new List<string>();
            StreamWriter writer = new StreamWriter("D:\\Dropbox\\Share\\Excels\\Indexer Data\\teams.txt");

            foreach (var team in teams)
            {
                writer.WriteLine(team.Value + " " + team.Key);
            }

            writer.Close();
        }

        public List<string> GetAllUniqueTeams(List<Result> results)
        {
            List<string> teams = new List<string>();

            foreach (var record in results)
            {
                if (!teams.Contains(record.HomeTeam)) teams.Add(record.HomeTeam);
                if (!teams.Contains(record.AwayTeam)) teams.Add(record.AwayTeam);
            }

            return teams;
        }

        public Dictionary<string, double> CalculateIndexChanges(List<int> dates, List<Result> results, List<string> teams)
        {
            Dictionary<string, double> indexMultipliers = new Dictionary<string, double>();

            foreach (var teamName in teams)
            {
                List<Result> matchesPlayed = GetResultsFromListByName(teamName, results);
                List<int> teamDates = new List<int>() { matchesPlayed.First().Date, matchesPlayed.Last().Date };
                double multiplier = CalculateIndexMultiplier(teamName, teamDates);
                indexMultipliers.Add(teamName, multiplier);
            }

            return indexMultipliers;
        }

        public double CalculateIndexMultiplier(string teamName, List<int> teamDates)
        {
            int startDate = teamDates.First();
            int endDate = teamDates.Last();
            Team firstTeam = database.Teams.Where(x => x.Name == teamName && x.LastUpdated <= startDate).OrderByDescending(x => x.Version).ToList()[1];
            Team lastTeam = database.Teams.Where(x => x.Name == teamName && x.LastUpdated == endDate).FirstOrDefault();
            return lastTeam.Idx / firstTeam.Idx;
        }

        public List<Result> GetResultsFromListByName(string teamName, List<Result> results)
        {
            return results.Where(x => x.HomeTeam == teamName || x.AwayTeam == teamName).ToList();
        }

        
    }
}
