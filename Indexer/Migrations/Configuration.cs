namespace Indexer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Indexer.POCOs;

    internal sealed class Configuration : DbMigrationsConfiguration<Indexer.Database>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Indexer.Database database)
        {
            database.Confederations.AddOrUpdate(
                x => x.Id,
                new Confederation { Name = "UEFA",     Version = 1, Index = 100, Active = true },
                new Confederation { Name = "CONMEBOL", Version = 1, Index = 100, Active = true },
                new Confederation { Name = "CONCACAF", Version = 1, Index = 100, Active = true },
                new Confederation { Name = "AFC",      Version = 1, Index = 100, Active = true },
                new Confederation { Name = "CAF",      Version = 1, Index = 100, Active = true },
                new Confederation { Name = "OFC",      Version = 1, Index = 50,  Active = true });

            database.ConfederationChanges.AddOrUpdate(
                x => x.Id,
                new ConfederationChange { Name = "Israel", Confederation = "", Date = 19730528 },
                new ConfederationChange { Name = "Israel", Confederation = "UEFA", Date = 19940904 },
                new ConfederationChange { Name = "Kazakhstan", Confederation = "UEFA", Date = 20040908 },
                new ConfederationChange { Name = "Australia", Confederation = "OFC", Date = 19800224 },
                new ConfederationChange { Name = "Australia", Confederation = "AFC", Date = 20060222 });

            database.Status.AddOrUpdate(
                x => x.Id,
                new Status { Date = 0, TotalGoalDifference = 0, TotalMatchesPlayed = 0, State = "" });
        }
    }
}
