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
        ConfederationHandler confHandler = new ConfederationHandler();
        ResultsHandler resultsHandler = new ResultsHandler();
        Dictionary<int, string> teamsToRetire = new Dictionary<int, string>()
        {
            { 19540328, "Saarland" },
            { 19750323, "Vietnam Republic" },
            { 19891115, "German DR" },
            { 19880622, "Yemen DPR" }
        };

        public Team GetTeam(string name, int date)
        {
            Team team = database.Teams.Where(x => x.Name == name && x.Active == true).FirstOrDefault();

            if (team == null) SetNewTeam(name, date, team);

            return team;
        }
        
        public void SetNewTeam(string name, int date, Team team)
        {
            double index;

            if (name == "Serbia") 
            {
                Team oldTeam = database.Teams.Where(x => x.Name == "Yugoslavia" && x.Active == true).FirstOrDefault();
                index = oldTeam.Index;
                oldTeam.Active = false;
            }
            else if (name == "Czech Republic")
            {
                Team oldTeam = database.Teams.Where(x => x.Name == "Czechoslovakia" && x.Active == true).FirstOrDefault();
                index = oldTeam.Index;
                oldTeam.Active = false;
            }
            else
            {
                index = confHandler.GetIndex(team.Confederation);
            }
            
            team.Name = name;
            team.LastUpdated = date;
            team.Version = 1;
            team.Confederation = confHandler.GetConfederationFromTxt(name);           
            team.Active = true;
            team.Index = Math.Round(index, 3);
            database.Teams.Add(team);
            database.SaveChanges();
        }

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
                        Index = member.Index * confMultipliers[confederation.Name]
                    });
                }

                database.SaveChanges();
            }
        }

        internal List<Team> GetTeamsByName(Dictionary<string, double>.KeyCollection keys)
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

        public bool isTeamActive(int date, string name)
        {
            bool result = true;

            foreach (var record in teamsToRetire)
            {
                if (teamsToRetire.ContainsKey(date) && teamsToRetire.ContainsValue(name)) result = false;
            }

            return result;
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
                List<Result> matchesPlayed = resultsHandler.GetResultsFromListByName(teamName, results);
                List<int> teamDates = new List<int>() { matchesPlayed.First().Date, matchesPlayed.Last().Date };
                double multiplier = CalculateIndexMultiplier(teamName, teamDates);
                indexMultipliers.Add(teamName, multiplier);
            }

            return indexMultipliers;
        }

        public double CalculateIndexMultiplier(string teamName, List<int> teamDates)
        {
            Team firstTeam = database.Teams.Where(x => x.Name == teamName && x.LastUpdated <= teamDates.First()).OrderByDescending(x => x.Version).ToList()[1];
            Team lastTeam = database.Teams.Where(x => x.Name == teamName && x.LastUpdated == teamDates.Last()).FirstOrDefault();
            return lastTeam.Index / firstTeam.Index;
        }
    }
}
