namespace Indexer
{
    using POCOs;
    using System.Data.Common;
    using System.Data.Entity;      
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class Database : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Confederation> Confederations { get; set; }
        public DbSet<ConfederationChange> ConfederationChanges { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<CurrentStatus> CurrentStatus { get; set; }

        public Database()
        {            
        }
        public Database(DbConnection connection)
            : base(connection, true)
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
