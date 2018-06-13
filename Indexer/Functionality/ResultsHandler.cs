using Indexer.Common;
using Indexer.POCOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer.Functionality
{
    public class ResultsHandler
    {
        Database database = new Database();
        TeamHandler teamHandler = new TeamHandler();
        ConfederationHandler confHandler = new ConfederationHandler();
        public void Run(Status status, int date)
        {
            List<Result> results = database.Results.Where(x => x.Date > status.Date && x.Date < date).ToList();
            string worldCupStatus = "pending";
            List<int> worldCupDates = new List<int>() { 0, 0 };

            foreach (var result in results)
            {
                worldCupStatus = UpdateWorldCupStatus(worldCupStatus, result, worldCupDates);

                Team homeTeam = teamHandler.GetTeam(result.HomeTeam, result.Date);
                Team awayTeam = teamHandler.GetTeam(result.AwayTeam, result.Date);
                double homeIndex = homeTeam.Index;
                double awayIndex = awayTeam.Index;

                if (result.Neutral == false)
                {
                    homeIndex += Constants.HOME_BONUS;
                    awayIndex -= Constants.HOME_BONUS;
                }

                int expectedDifference = (int) Math.Ceiling((homeIndex - awayIndex) / 15);
                expectedDifference = expectedDifference > Constants.MAX_GOAL_DIFF ? Constants.MAX_GOAL_DIFF : expectedDifference;                
                int actualDifference = result.HomeGoals - result.AwayGoals;
                int difference = actualDifference - expectedDifference;
                double indexChange = 5.0 * (difference);
                status.Date = result.Date;
                status.TotalGoalDifference += difference;
                status.TotalMatchesPlayed++;

                Team newHomeTeam = new Team()
                {
                    Name = homeTeam.Name,
                    Confederation = homeTeam.Confederation,
                    LastUpdated = result.Date,
                    Version = homeTeam.Version++,
                    Index = Math.Round(homeIndex + indexChange, 3),
                    Active = teamHandler.isTeamActive(result.Date, homeTeam.Name)
                };

                Team newAwayTeam = new Team()
                {
                    Name = awayTeam.Name,
                    Confederation = awayTeam.Confederation,
                    LastUpdated = result.Date,
                    Version = awayTeam.Version++,
                    Index = Math.Round(awayIndex - indexChange, 3),
                    Active = teamHandler.isTeamActive(result.Date, awayTeam.Name)
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
                confHandler.UpdateConfederations(dates);
                status = "pending";
            }

            return status;
        }

        public List<Result> GetWorldCupResults(List<int> dates)
        {
            return database.Results.Where(x => x.Competition == "FIFA World Cup" && x.Date >= dates[0] && x.Date <= dates[1]).ToList();
        }

        public List<Result> GetResultsFromListByName(string teamName, List<Result> results)
        {
            return results.Where(x => x.HomeTeam == teamName || x.AwayTeam == teamName).ToList();
        }
    }
}
