using Indexer.Common;
using Indexer.POCOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer.Functionality
{
    public class ResultsHandler
    {
        Database database = new Database();
        ConfederationHandler confHandler = new ConfederationHandler();
        Dictionary<int, string> teamsToRetire = new Dictionary<int, string>()
        {
            { 19540328, "Saarland" },
            { 19750323, "Vietnam Republic" },
            { 19891115, "German DR" },
            { 19880622, "Yemen DPR" }
        };
        public void Run(CurrentStatus status, int date)
        {
            List<Result> results = database.Results.Where(x => x.Date > status.Date && x.Date < date).ToList();
            string worldCupStatus = "pending";
            List<int> worldCupDates = new List<int>() { 0, 0 };

            foreach (var result in results)
            {
                worldCupStatus = UpdateWorldCupStatus(worldCupStatus, result, worldCupDates);

                Team homeTeam = GetTeam(result.HomeTeam, result.Date);
                Team awayTeam = GetTeam(result.AwayTeam, result.Date);
                double homeIndex = homeTeam.Idx;
                double awayIndex = awayTeam.Idx;

                if (result.Neutral == false)
                {
                    homeIndex += Constants.HOME_BONUS;
                    awayIndex -= Constants.HOME_BONUS;
                }

                int expectedDifference = (int)Math.Ceiling((homeIndex - awayIndex) / 15);
                expectedDifference = expectedDifference > Constants.MAX_GOAL_DIFF ? Constants.MAX_GOAL_DIFF : expectedDifference;
                int actualDifference = result.HomeGoals - result.AwayGoals;
                int difference = actualDifference - expectedDifference;
                double idxDiffCoeff = (1 - Math.Abs(homeTeam.Idx - awayTeam.Idx) / 200);
                double totalIdxCoeff = Math.Abs((Math.Abs(homeTeam.Idx + awayTeam.Idx - 200) - 200) / 200);
                double indexChange = 5.0 * (difference) * idxDiffCoeff * totalIdxCoeff;
                status.Date = result.Date;
                status.TotalGoalDifference += difference;
                status.TotalMatchesPlayed++;

                Team newHomeTeam = new Team()
                {
                    Name = homeTeam.Name,
                    Confederation = homeTeam.Confederation,
                    LastUpdated = result.Date,
                    Version = homeTeam.Version++,
                    Idx = Math.Round(homeIndex + indexChange, 3),
                    Active = isTeamActive(result.Date, homeTeam.Name)
                };

                Team newAwayTeam = new Team()
                {
                    Name = awayTeam.Name,
                    Confederation = awayTeam.Confederation,
                    LastUpdated = result.Date,
                    Version = awayTeam.Version++,
                    Idx = Math.Round(awayIndex - indexChange, 3),
                    Active = isTeamActive(result.Date, awayTeam.Name)
                };

                result.ExpectedDifference = expectedDifference;
                database.Teams.Add(newHomeTeam);
                database.Teams.Add(newAwayTeam);
                homeTeam.Active = false;
                awayTeam.Active = false;
                database.SaveChanges();
            }
        }

        public string UpdateWorldCupStatus(string status, Result result, List<int> dates)
        {
            if (result.Competition == "FIFA World Cup")
            {
                status = "ongoing";
                if (dates[0] == 0) dates[0] = result.Date;
                dates[1] = result.Date;
            }

            if (result.Competition != "FIFA World Cup" && status == "ongoing") return "done";

            if (result.Competition != "FIFA World Cup" && status == "done")
            {
                //confHandler.UpdateConfederations(dates);
                dates[0] = 0;
                dates[1] = 0;
                status = "pending";
            }

            return status;
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

        public Team GetTeam(string name, int date)
        {
            Team team = database.Teams.Where(x => x.Name == name && x.Active == true).FirstOrDefault();

            if (team == null)  team = SetNewTeam(name, date);

            return team;
        }

        public Team SetNewTeam(string name, int date)
        {
            double index;
            Team team = new Team();

            if (name == "Serbia")
            {
                Team oldTeam = database.Teams.Where(x => x.Name == "Yugoslavia" && x.Active == true).FirstOrDefault();
                index = oldTeam.Idx;
                team.Confederation = oldTeam.Confederation;
                oldTeam.Active = false;
            }
            else if (name == "Czech Republic")
            {
                Team oldTeam = database.Teams.Where(x => x.Name == "Czechoslovakia" && x.Active == true).FirstOrDefault();
                index = oldTeam.Idx;
                team.Confederation = oldTeam.Confederation;
                oldTeam.Active = false;
            }
            else
            {
                team.Confederation = GetConfederationFromTxt(name);
                index = GetIndex(team.Confederation);
            }

            database.SaveChanges();
            team.Name = name;
            team.LastUpdated = date;
            team.Version = 1;            
            team.Active = true;
            team.Idx = Math.Round(index, 3);
            database.Teams.Add(team);            

            return team;
        }

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
    }
}
